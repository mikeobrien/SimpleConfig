require "albacore"
require_relative "filesystem"
require_relative "gallio-task"

reportsPath = "reports"
version = ENV["BUILD_NUMBER"]

task :build => :pushLocalPackage
task :deploy => :pushPublicPackage

assemblyinfo :assemblyInfo do |asm|
    asm.version = version
    asm.company_name = "Ultraviolet Catastrophe"
    asm.product_name = "Simple Config"
    asm.title = "Simple Config"
    asm.description = "Simple Configuration for .NET"
    asm.copyright = "Copyright (c) #{Time.now.year} Ultraviolet Catastrophe"
    asm.output_file = "src/SimpleConfig/Properties/AssemblyInfo.cs"
end

msbuild :buildLibrary => :assemblyInfo do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/SimpleConfig/SimpleConfig.csproj"
end

msbuild :buildTests => :buildLibrary do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/Tests/Tests.csproj"
end

task :unitTestInit do
	FileSystem.EnsurePath(reportsPath)
end

gallio :unitTests => [:buildTests, :unitTestInit] do |runner|
	runner.echo_command_line = true
	runner.add_test_assembly("src/Tests/bin/Release/Tests.dll")
	runner.verbosity = 'Normal'
	runner.report_directory = reportsPath
	runner.report_name_format = 'tests'
	runner.add_report_type('Html')
end

nugetApiKey = ENV["NUGET_API_KEY"]
deployPath = "deploy"
artifactsPath = 'artifacts'

packagePath = File.join(deployPath, "package")
nuspecFilename = "SimpleConfig.nuspec"
packageLibPath = File.join(packagePath, "lib")
binPath = "src/SimpleConfig/bin/release"
packageFilePath = File.join(deployPath, "SimpleConfig.#{version}.nupkg")

task :prepPackage => :unitTests do
	FileSystem.DeleteDirectory(deployPath)
	FileSystem.EnsurePath(packageLibPath)
	FileSystem.DeleteDirectory(artifactsPath)
    FileSystem.EnsurePath(artifactsPath)
	FileSystem.CopyFiles(File.join(binPath, "SimpleConfig.dll"), packageLibPath)
	FileSystem.CopyFiles(File.join(binPath, "SimpleConfig.pdb"), packageLibPath)
end

nuspec :createSpec => :prepPackage do |nuspec|
   nuspec.id = "SimpleConfig"
   nuspec.version = version
   nuspec.authors = "Mike O'Brien"
   nuspec.owners = "Mike O'Brien"
   nuspec.title = "Simple Config"
   nuspec.description = "Simple Configuration for .NET"
   nuspec.summary = "Simple Configuration for .NET"
   nuspec.language = "en-US"
   nuspec.licenseUrl = "https://github.com/mikeobrien/SimpleConfig/blob/master/LICENSE"
   nuspec.projectUrl = "https://github.com/mikeobrien/SimpleConfig"
   nuspec.iconUrl = "https://github.com/mikeobrien/SimpleConfig/raw/master/misc/logo.png"
   nuspec.working_directory = packagePath
   nuspec.output_file = nuspecFilename
   nuspec.tags = "configuration xml"
   nuspec.dependency "Bender", "1.0.12.0"
end

nugetpack :createPackage => :createSpec do |nugetpack|
   nugetpack.nuspec = File.join(packagePath, nuspecFilename)
   nugetpack.base_folder = packagePath
   nugetpack.output = deployPath
end

task :pushLocalPackage => :createPackage do
	FileSystem.CopyFiles(packageFilePath, artifactsPath)
end

nugetpush :pushPublicPackage => :createPackage do |nuget|
    nuget.apikey = nugetApiKey
    nuget.package = packageFilePath.gsub('/', '\\')
end