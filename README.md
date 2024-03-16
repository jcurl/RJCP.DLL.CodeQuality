# CodeQuality <!-- omit in toc -->

The `CodeQuality` library is a set of extra tools to support other types of
testing with the NUnit Framework than just Unit Testing.

You can deploy files, get the current work directory and implement accessors (in
most cases) for internal and private classes.

It is intended for use with NUnit 3.x and 4.x and has backwards compatibility to
NUnit 2.7.1 for older legacy projects.

There is no direct reference from `CodeQuality.NUnitExtensions` to the
`NUnit.Framework` itself, so it will use the version of the library you choose
for your project.

- [1. NUnit Extensions Library Compatibility](#1-nunit-extensions-library-compatibility)
- [2. NUnit Extensions File Based Testing](#2-nunit-extensions-file-based-testing)
  - [2.1. The Test Directory and Work Directory](#21-the-test-directory-and-work-directory)
    - [2.1.1. The TestDirectory](#211-the-testdirectory)
    - [2.1.2. The WorkDirectory](#212-the-workdirectory)
  - [2.2. Deployment for Integration Tests](#22-deployment-for-integration-tests)
    - [2.2.1. Deploying Files for Reading Only](#221-deploying-files-for-reading-only)
    - [2.2.2. Deploy Files for Modification and Writing](#222-deploy-files-for-modification-and-writing)
    - [2.2.3. Creating a New File](#223-creating-a-new-file)
    - [2.2.4. The Current Directory](#224-the-current-directory)
      - [2.2.4.1. The Current Directory for NUnit 2.7.1](#2241-the-current-directory-for-nunit-271)
      - [2.2.4.2. The Current Directory for NUnit 3.x and 4.x](#2242-the-current-directory-for-nunit-3x-and-4x)
      - [2.2.4.3. Comparison between NUnit 2.7.1 and 3.x](#2243-comparison-between-nunit-271-and-3x)
      - [2.2.4.4. NUnit 2.x GUI Workaround](#2244-nunit-2x-gui-workaround)
    - [2.2.5. Configuration of NUnitExtensions WorkDirectory](#225-configuration-of-nunitextensions-workdirectory)
      - [2.2.5.1. Setting a Different Work Directory to the Test Directory](#2251-setting-a-different-work-directory-to-the-test-directory)
      - [2.2.5.2. Forcing a Different Work Directory](#2252-forcing-a-different-work-directory)
      - [2.2.5.3. 2.2.5.3 Setting the Work Directory to the Current Directory](#2253-2253-setting-the-work-directory-to-the-current-directory)
    - [2.2.6. Deploy Operations](#226-deploy-operations)
      - [2.2.6.1. Creating a New Directory](#2261-creating-a-new-directory)
      - [2.2.6.2. Deleting a Directory](#2262-deleting-a-directory)
      - [2.2.6.3. Deleting a File](#2263-deleting-a-file)
    - [2.2.7. Deploy Context and some Internal Details](#227-deploy-context-and-some-internal-details)
    - [2.2.8. Tips and Check Lists in Porting NUnit 2 to NUnit 3](#228-tips-and-check-lists-in-porting-nunit-2-to-nunit-3)
      - [2.2.8.1. Use WorkDirectory as CurrentDirectory until Migration](#2281-use-workdirectory-as-currentdirectory-until-migration)
      - [2.2.8.2. Changes in Deploy for NUnitExtensions v0.4.0](#2282-changes-in-deploy-for-nunitextensions-v040)
      - [2.2.8.3. Deploy.Item](#2283-deployitem)
      - [2.2.8.4. Look for TestContext](#2284-look-for-testcontext)
      - [2.2.8.5. TestDirectory should be Read Only, WorkDirectory is a RAM Disk](#2285-testdirectory-should-be-read-only-workdirectory-is-a-ram-disk)
      - [2.2.8.6. Change the Environment.CurrentDirectory](#2286-change-the-environmentcurrentdirectory)
  - [2.3. Differences Between .NET Framework and .NET Core](#23-differences-between-net-framework-and-net-core)
- [3. Accessors](#3-accessors)
- [4. INI Configuration Files](#4-ini-configuration-files)
  - [4.1. An Example of Multiple Configurations](#41-an-example-of-multiple-configurations)
- [5. Logging](#5-logging)
  - [5.1. .NET Core ILogger](#51-net-core-ilogger)
- [6. References](#6-references)
- [7. Release History](#7-release-history)
  - [Version 0.x](#version-0x)
  - [7.1. Version 0.6.1](#71-version-061)
  - [7.2. Version 0.6.0](#72-version-060)
  - [7.3. Version 0.5.1.1](#73-version-0511)
  - [7.4. Version 0.5.1](#74-version-051)
  - [7.5. Version 0.5.0.1](#75-version-0501)
  - [7.6. Version 0.5.0](#76-version-050)
  - [7.7. Version 0.4.3](#77-version-043)
  - [7.8. Version 0.4.2](#78-version-042)
  - [7.9. Version 0.4.1](#79-version-041)
  - [7.10. Version 0.4.0](#710-version-040)
  - [7.11. Version 0.3.0](#711-version-030)
  - [7.12. Version 0.2.1](#712-version-021)
  - [7.13. Version 0.2.0](#713-version-020)
  - [7.14. Version 0.1.0](#714-version-010)

## 1. NUnit Extensions Library Compatibility

This library is intended to work with NUnit 2.6.x and NUnit 3.x and 4.x. Even
though the libraries are incompatible with each other, there are many
similarities that still make NUnitExtensions work.

There is no compile time dependency from this library to NUnit. The dependencies
are resolved at run time.

## 2. NUnit Extensions File Based Testing

### 2.1. The Test Directory and Work Directory

#### 2.1.1. The TestDirectory

NUnit has a `TestContext.CurrentContext.TestDirectory`. This is the location
where the assembly being tested is stored.

The `CodeQuality.NUnitExtensions` has a method called `Deploy.TestDirectory`,
which is similar. When determining the directory of the assembly being tested,
use `Deploy.TestDirectory` instead. It's use will be apparent in conjunction
with the `Deploy.WorkDirectory`.

#### 2.1.2. The WorkDirectory

NUnit has a `TestContext.CurrentContext.WorkDirectory`. This is usually set by
the runner. For example, NUnit 3.x Console Runner has the option `--work` that
specifies the work directory, setting this property (the NUnit 2.x console
runner has the `/work` option).

The equivalent in `CodeQuality.NUnitExtensions` is `Deploy.WorkDirectory`. This
may be different to the `CurrentContext.WorkDirectory` by `NUnit.Framework`, to
support integration tests. It is the working directory where test cases should
write to.

### 2.2. Deployment for Integration Tests

Many test cases may need to test functions that read and write files. In your
test assembly, you would include a file in to the project and specify it as:

- Content: None
- Copy If Newer

That would copy the file from your project directory to the directory where your
assembly is, using the directory structure specified in your project. These
files form inputs to your integration test cases.

There are two scenarios where you would want to work on these files: to read; or
to modify and write (including writing new files based on the inputs).

#### 2.2.1. Deploying Files for Reading Only

When you want to test a function that only reads a file, there is no need to
deploy it and copy it a second time. Just read it in place. Assuming that a file
might be called `TestResources/test.txt`, and that file is copied during the
project build, your test code might look like:

```csharp
private readonly string FilePath = Path.Combine(Deploy.TestDirectory, "TestResources", "test.txt");

[Test]
public void MyTestFunction()
{
    MyTestAssembly.TestFunction(FilePath);
}
```

#### 2.2.2. Deploy Files for Modification and Writing

Some times it is necessary to test a function that reads a file, where it might
modify it, or might create new files relative to the path of the original file
(e.g. metadata files might be created, or configuration files related to the
input file might be created or log files might be created).

Test cases should not modify the contents with in the `Deploy.TestDirectory`
directory. Files should be copied from the `Deploy.TestDirectory` to the
`Deploy.WorkDirectory` first before they are modified.

Assuming that a file might be called `TestResources/test.txt` which needs to be
opened, modified, and written back, a following test case might look like:

```csharp
[Test]
public void MyTestModifyFunction()
{
    Deploy.Item("TestResources/test.txt", "Test");
    string workPath = Path.Combine(Deploy.WorkDirectory, "Test", "test.txt");

    MyTestAssembly.ReadAndModify(workPath);
}
```

The function `Deploy.Item()` will copy from the path given relative to the
`Deploy.TestDirectory`, to the path given relative to the
`Deploy.WorkDirectory`.

#### 2.2.3. Creating a New File

When a function needs a path to write to a file, always give it relative to the
`Deploy.WorkDirectory`. For example:

```csharp
[Test]
public void MyWriteTest()
{
    string path = Path.Combine(Deploy.WorkDirectory, "out.txt");
    MyTestAssembly.Save(path);
}
```

#### 2.2.4. The Current Directory

Do not assume any current value for the current directory, as it changes
depending on the test context.

In NUnit 2.x, the current directory was set to be the same as the
`TestContext.CurrentContext.CurrentDirectory`. In NUnit 3.x it is no longer set
(see [NUnit 3.x issue #1072](https://github.com/nunit/nunit/issues/1072)). The
authors of NUnit indicate that this change is by design, and is documented at
[Breaking Changes](https://github.com/nunit/docs/wiki/Breaking-Changes).

As an experiment, the following tests were done:

```csharp
namespace NUnitTest
{
    using System;
    using NUnit.Framework;

    [TestFixture(Category = "TestFixture")]
    public class Class1
    {
        [Test]
        public void Directories()
        {
            Console.WriteLine("Current Directory: {0}", Environment.CurrentDirectory);
            Console.WriteLine("Context Test Dir:  {0}", TestContext.CurrentContext.TestDirectory);
            Console.WriteLine("Context Work Dir:  {0}", TestContext.CurrentContext.WorkDirectory);
        }
    }
}
```

##### 2.2.4.1. The Current Directory for NUnit 2.7.1

When running in Visual Studio 2015 IDE:

```text
Current Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Test Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Work Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
```

When running using `nunit-console.exe` from the same directory as the test:

```cmd
$ nunit-console.exe .\NUnit2Test.dll
Current Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Test Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Work Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
```

When running using `nunit-console.exe` from a different directory as the test:

```cmd
$ pwd
<mydocuments>\NUnit2Test\NUnit2Test\bin

$ nunit-console.exe .\Debug\NUnit2Test.dll
Current Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Test Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Work Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin
```

When running using `nunit-console.exe` from the same directory as the test with
`/work`:

```cmd
$ nunit-console.exe /work:nunit-out .\NUnit2Test.dll
Current Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Test Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Work Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug\nunit-out
```

You will notice that the values of `CurrentDirectory` and `TestDirectory` are
the same. This is set by the nunit2 runner.

You will also notice that the work directory is based on the directory where the
console runner is started, except if the `/work` command line option is
provided.

##### 2.2.4.2. The Current Directory for NUnit 3.x and 4.x

When running in Visual Studio 2015 IDE:

```text
Current Directory: <programfiles>\Microsoft Visual Studio 14.0\Common7\IDE
Context Test Dir:  <mydocuments>\NUnit3Test\NUnit3Test\bin\Debug
Context Work Dir:  <mydocuments>\NUnit3Test\NUnit3Test\bin\Debug
```

When running using `nunit3-console.exe` from the packages directory:

```cmd
$ pwd
<mydocuments>\NUnit3Test\packages

$ nunit3-console.exe ..\..\..\NUnit3Test\bin\Debug\NUnit3Test.dll
Current Directory: <mydocuments>\NUnit3Test\packages\NUnit.ConsoleRunner.3.10.0\tools
Context Test Dir:  <mydocuments>\NUnit3Test\NUnit3Test\bin\Debug
Context Work Dir:  <mydocuments>\NUnit3Test\packages\NUnit.ConsoleRunner.3.10.0\tools
```

When running `nunit3-console.exe` from a different directory:

```cmd
$ pwd
<mydocuments>\NUnit3Test\packages

$ tools\nunit3-console.exe ..\..\NUnit3Test\bin\Debug\NUnit3Test.dll
Current Directory: <mydocuments>\NUnit3Test\packages\NUnit.ConsoleRunner.3.10.0
Context Test Dir:  <mydocuments>\NUnit3Test\NUnit3Test\bin\Debug
Context Work Dir:  <mydocuments>\NUnit3Test\packages\NUnit.ConsoleRunner.3.10.0
```

When running `nunit3-console.exe` with shadowing:

```cmd
$ pwd
<mydocuments>\NUnit3Test\packages

$ nunit3-console.exe --shadowcopy ..\..\..\NUnit3Test\bin\Debug\NUnit3Test.dll
Current Directory: <mydocuments>\NUnit3Test\packages\NUnit.ConsoleRunner.3.10.0\tools
Context Test Dir:  <mydocuments>\NUnit3Test\NUnit3Test\bin\Debug
Context Work Dir:  <mydocuments>\NUnit3Test\packages\NUnit.ConsoleRunner.3.10.0\tools
```

##### 2.2.4.3. Comparison between NUnit 2.7.1 and 3.x

You will see that:

| NUnit Environment                | Current Directory                            | Test Directory                | Work Directory                          |
| -------------------------------- | -------------------------------------------- | ----------------------------- | --------------------------------------- |
| 2.7.1 (Visual Studio)            | Same as `TestDirectory`                      | Location of the Test Assembly | Same as `TestDirectory`                 |
| 2.7.1 (Console Runner)           | Same as `TestDirectory`                      | Location of the Test Assembly | The directory where the test is started |
| 2.7.1 (Console Runner `/work`)   | Same as `TestDirectory`                      | Location of the Test Assembly | As given by `/work`                     |
| 3.11.0 (Visual Studio)           | Visual Studio Program Files IDE Installation | Location of the Test Assembly | Same as `TestDirectory`                 |
| 3.11.0 (Console Runner)          | The directory where the test is started      | Location of the Test Assembly | The directory where the test is started |
| 3.11.0 (Console Runner `--work`) | The directory where the test is started      | Location of the Test Assembly | As given by `--work`                    |

Thus, one can see clearly:

- Do not rely on the current directory, if test cases are to be compatible
  between NUnit 2 and 3.
- Use `Deploy.TestDirectory` to get the path to where the test assembly is, even
  if shadowing is enabled.
- Use `Deploy.WorkDirectory` to get the path where the user wants to put results
  and temporary files.

##### 2.2.4.4. NUnit 2.x GUI Workaround

When starting the NUnit 2.x GUI from Windows, the
`TestContext.CurrentContext.WorkDirectory` is set to be the current directory
when it is started.

Go to the Start Menu icon for your Operating System, select its properties and
go to the *Shortcut tab*.

- Target: `"C:\Program Files (x86)\NUnit 2.6.4\bin\nunit.exe"`
- Start In: `"C:\Program Files (x86)\NUnit 2.6.4\bin\"`

This causes a problem when running test cases.

```text
Current Directory: <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Context Test Dir:  <mydocuments>\NUnit2Test\NUnit2Test\bin\Debug
Context Work Dir:  C:\Program Files (x86)\NUnit 2.6.4\bin
```

That is, a program would attempt to deploy files from `TestDirectory` which is
correct, but to a `WorkDirectory` which is read-only.

One should modify the property `Start In` to be a temporary directory (or a RAM
disk):

- Target: `%TEMP%\NUnit2`

An example of a useful RAM Disk driver is
[ImDisk](http://www.ltr-data.se/opencode.html/#ImDisk).

Or the `NUnit.exe` should be started from the command line.

#### 2.2.5. Configuration of NUnitExtensions WorkDirectory

As seen earlier, running unit tests within the Visual Studio IDE has
`TestDirectory` and `WorkDirectory` being the same, but when running on the
command line, a user is very likely to set the `WorkDirectory` to something
else. This may lead to a user inadvertently using `TestDirectory` as the base to
read from a file after using `Deploy.Item`, where `WorkDirectory` is required
instead.

##### 2.2.5.1. Setting a Different Work Directory to the Test Directory

To capture these errors easier in the Visual Studio IDE, the NUnitExtensions
supports a configuration:

File: `app.config`

```xml
<configSections>
  <section name="NUnitExtensions" type="RJCP.CodeQuality.AppConfig.NUnitExtensionsSection, RJCP.CodeQuality"/>
</configSections>

<NUnitExtensions>
  <deploy workDir="work"/>
</NUnitExtensions>
```

This configuration tells `NUnitExtensions` to set `Deploy.WorkDirectory` to be
the `TestContext.CurrentContext.WorkDirectory` with the extension of the folder
`work`. It will only do this in the example given, if `TestDirectory` and
`WorkDirectory` would otherwise be the same.

Within the Visual Studio IDE, it could be easier detected through a
`FileNotFoundException` if after deploying a file, it is read from the wrong
location.

##### 2.2.5.2. Forcing a Different Work Directory

In the case that you wish to enforce a different work directory always, you can
provide the `force` attribute.

File: `app.config`

```xml
<configSections>
  <section name="NUnitExtensions" type="RJCP.CodeQuality.AppConfig.NUnitExtensionsSection, RJCP.CodeQuality"/>
</configSections>

<NUnitExtensions>
  <deploy workDir="R:\work" force="true"/>
</NUnitExtensions>
```

##### 2.2.5.3. 2.2.5.3 Setting the Work Directory to the Current Directory

In porting existing test cases written and working under NUnit 2 which rely on
the current directory being the same as the test directory, it is possible to
set the work directory to be the same as the current directory. Under NUnit 2.x,
this sets the work directory to be the `TestDirectory` which is also the
`Environment.CurrentDirectory`.

File: `app.config`

```xml
<configSections>
  <section name="NUnitExtensions" type="RJCP.CodeQuality.AppConfig.NUnitExtensionsSection, RJCP.CodeQuality"/>
</configSections>

<NUnitExtensions>
  <deploy useCwd="true"/>
</NUnitExtensions>
```

#### 2.2.6. Deploy Operations

##### 2.2.6.1. Creating a New Directory

You can create a new directory relative to `Deploy.WorkDirectory` with the
method `Deploy.CreateDirectory()`. This will create a directory for you.

##### 2.2.6.2. Deleting a Directory

A test case might need to ensure that a directory doesn't exist. You can remove
a directory relative to `Deploy.WorkDirectory` with the method
`Deploy.DeleteDirectory()`. This method does additional work to ensure that the
directory is deleted by checking for its non-existence after deletion. On
Windows, directories may not be deleted immediately if it there is a file open
inside (such as a temporary option by a Virus Scanner or a Shell Extension), and
so a retries are performed.

##### 2.2.6.3. Deleting a File

It may be necessary to remove a file in a test case. You can remove a file
relative to the `Deploy.WorkDirectory` with the method `Deploy.DeleteFile()`.
This method does additional work to ensure that the file is deleted by checking
for its non-existence after deletion. On Windows, files may not be deleted
immediately if it there is that file is open (such as a temporary option by a
Virus Scanner or a Shell Extension), and so retries are performed.

#### 2.2.7. Deploy Context and some Internal Details

The `CodeQuality.NUnitExtensions` uses reflection to get the version of the
`NUnit.Framework` which is in the class. This requires that the call stack where
methods inside of the `Deploy` class are called are done so within a function
having one of the following two attributes:

- `TestAttribute`
- `TestCaseAttribute`

or where one of the frames of the call stack has a class explicitly marked with:

- `TestFixtureAttribute`

When it calculates what the `Deploy.TestDirectory` or the
`Deploy.WorkDirectory`, it iterates through its call stack to look for the first
method with one of these two attributes. Every test case executed by NUnit must
have one of these attributes. The assembly to which this attribute belongs tells
`CodeQuality.NUnitExtensions` which version of NUnit the user is using, and can
then get the class from the `TestContext.CurrentContext` property.

This necessarily means, that deployment using the `Deploy` class should be done
from any code that is not directly called from the NUnit test runner.

The results of reflection are cached for the duration of testing for slightly
better performance. Any changes to NUnit's `TestContext.CurrentContext` for
`TestDirectory` or `WorkDirectory` after the first call to `Deploy` will likely
not be noticed.

#### 2.2.8. Tips and Check Lists in Porting NUnit 2 to NUnit 3

##### 2.2.8.1. Use WorkDirectory as CurrentDirectory until Migration

For initial code when using `NUnitExtensions` v0.4.0, populate your
configuration with:

`app.config`

```xml
<NUnitExtensions>
  <deploy useCwd="true"/>
</NUnitExtensions>
```

until you're ready to migrate.

##### 2.2.8.2. Changes in Deploy for NUnitExtensions v0.4.0

Do not assume the value of `Environment.CurrentDirectory` is equivalent to
`TestContext.CurrentContext.TestDirectory`. Test cases that do this will likely
fail.

In version v0.3.0 of `NUnitExtensions`, the `Deploy` class assumed relative
paths are from the current directory. This is no longer the case from v0.4.0 and
later, source paths in `Deploy.Item()` are relative to `Deploy.TestDirectory`,
and destination paths are relative to `Deploy.WorkDirectory`.

##### 2.2.8.3. Deploy.Item

Do a search in your code for `Deploy.Item()` and evaluate the logic that it
doesn't depend on the current directory.

If your code only reads files for testing, create a path

```csharp
string path = Path.Combine(Deploy.TestDirectory, "path.txt");
```

If you need to combine multiple files from the `TestDirectory`, then deploy:

```csharp
Deploy.Item("TestResources/Config/config.xml", "test1");
Deploy.Item("logs/V850/Trace.tr", "test1");
Deploy.Item("logs/V850/Database", "test1");
```

If files are created in the test case and are dependent on the configuration
file path, you should deploy, so that your test code doesn't write to the
`TestDirectory`.

```csharp
Deploy.Item("TestResources/testConfig.xml", "test2");

string inPath = Path.Combine(Deploy.WorkDirectory, "test2", "testConfig.xml");
Framework.ParseFile(inPath);
```

##### 2.2.8.4. Look for TestContext

Search if your code already uses NUnit's `TestContext.CurrentContext` property,
and change instances of `TestDirectory` and `WorkDirectory` to use the
implementation provided by the `Deploy.TestDirectory` and
`Deploy.WorkDirectory`.

##### 2.2.8.5. TestDirectory should be Read Only, WorkDirectory is a RAM Disk

You might want run test cases where the `TestDirectory` is read only. Any
attempt to write to the `TestDirectory` should cause your test cases to fail. As
well as this, create a RAM Disk for the `WorkDirectory` when using the NUnit
GUI.

A useful and free Windows RAM Disk driver is
[ImDisk](http://www.ltr-data.se/opencode.html/#ImDisk). Once you've set up a RAM
disk (say `R:`), modify the start up directory to be `R:`, or start `nunit.exe`
from a command prompt from the `R:` drive.

To check for test cases that write to the `TestDirectory` on Windows, create a
new share of your project directory, making it read only (ensure it's shared
only with yourself, unless you want others to see it).

Then map that share to a new drive path (say `\\localhost\project` to `M:`). Use
NUnit GUI to load the test assemblies from the read only share, in parallel
writing the code using Visual Studio.

When you run test cases, if they attempt to write to the `TestDirectory`,
they'll generally fail with a `System.UnauthorizedException` as the file system
is read only.

##### 2.2.8.6. Change the Environment.CurrentDirectory

If in doubt, modify the test case, at least temporarily, so that the current
directory is something else (e.g. instead of it being the default value of
`TestDirectory`, change it to be something read only, such as `M:\`). Some test
cases might still read from the current directory, which would break when
migrating to NUnit 3.

### 2.3. Differences Between .NET Framework and .NET Core

The assembly supports reading the configuration file above from the test
assemblies `app.config` file (e.g. if your assembly is called `MyTest.dll`, then
the application configuration file is deployed as `MyTest.dll.config`).

In .NET 4.x, this is part of the framework. For .NET Core (.NET Standard as
well), the configuration libraries don't look for `MyTest.dll.config`, but
instead try to resolve to `testhose.exe.config` which normally doesn't work (and
is different to .NET Framework).

The first time that `CodeQuality.NUnitExtensions` tries to look for the
configuration for .NET Core projects, it will iterate through the call stack for
the assemblies used and try to identify the configuration file. You should only
have one configuration file for your test project.

## 3. Accessors

It is possible to use the `CodeQuality` library for implementing "accessors",
which are classes to expose functionality otherwise private or internal of the
assembly being tested.

There is a set of detailed instructions providing a cookbook on how to write
accessors.

## 4. INI Configuration Files

The `CodeQuality` has a namespace called `Config` that has a simple
implementation of reading configuration files based on Windows INI file format.

A test case may require that a user provide some configurable settings (and it
will use a set of predefined defaults).  These settings may not be preferable to
put in the `app.config` file, as it is generally harder to deploy the
`app.config` based on a build type.

For example, when testing using the Visual Studio IDE, test case settings may be
using the default. When testing on the command line, the build scripts outside
of Visual Studio IDE may provide configuration files providing details for the
tests. It is much easier to deploy a new configuration file `testconfig.ini`
than it is to clutter the application code `app.config` with all the various
configurations.

### 4.1. An Example of Multiple Configurations

Let's take the example that a test assembly wishes to test a sockets
implementation. A server is implemented which listens to a specific address
(let's assume localhost) and a specific port. The tests take some time, and the
build system may decide to run multiple test runners as results of multiple
builds in parallel, to reduce the overall test time (the test cases are I/O
bound, not CPU bound, and waits most of the time).

The project may decide that the default port is 4080, but when running two of
the assemblies in parallel., one instance should use the listening port 4080 and
the other 4081.

An NUnit Test Assembly may implement a configuration class:

```csharp
namespace RJCP.Net.Configuration
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.Config;

    public static class TestConfig
    {
        private static string ConfigFileName
        {
            get
            {
                return System.IO.Path.Combine(Deploy.TestDirectory, "config.ini");
            }
        }

        public static int TcpTerminalListenPort
        {
            get
            {
                return IniFile.GetKey(ConfigFileName, "TcpTerminal", "ListenPort", 4080);
            }
        }
    }
}
```

The test case would then query the port inside the test case:

```csharp
[Test]
public void ConnectToListener()
{
    using (TestListener listener = new TestListener("127.0.0.1", Configuration.TestConfig.TcpTerminalListenPort)) {
        var server = new MyClient(Configuration.TestConfig.TcpTerminalListenPort);
        server.Connect();
    }
}
```

There would be no file `config.ini` available when running within Visual Studio,
thus returning the default value `4080` in the example given. A build script
could build two different images (e.g. one in Debug mode, the other in Release
mode, with the build results in two different directories), and copy the file
`config-Debug.ini` or `config-Release.ini` to the appropriate build directories
as `config.ini`.

The content of one of the files might then be:

```ini
[TcpTerminal]
ListenPort=4081
```

while the other is

```ini
[TcpTerminal]
ListenPort=4082
```

and thus allow the `nunit-console.exe` to run simultaneously for both assemblies
without both test runs trying to create a listen server on the same port
simultaneously (that would usually result in a failure).

## 5. Logging

### 5.1. .NET Core ILogger

This library also provides an implementation of an `ILogger` that you can pass
to your test cases. If you tried to use a `ConsoleLogger` from .NET Core, you'd
find [1] that perhaps the first test case logged correctly, but subsequent
logging was missing. The analysis of [1] also describes why it doesn't work.

When you build your logger factory, use the extension method `AddNUnitLogger()`
as in the following code sample:

```csharp
using Microsoft.Extensions.Logging;
using RJCP.CodeQuality.NUnitExtensions.Trace;

[TestFixture]
public class NUnitLoggerTest {
  private static ILoggerFactory s_LoggerFactory;
  private static readonly object s_Lock = new object();

  public static ILoggerFactory LoggerFactory {
    get {
      if (s_LoggerFactory == null) {
        lock (s_Lock) {
          if (s_LoggerFactory == null) {
            s_LoggerFactory = LoggerFactory.Create(builder => {
              builder.AddFilter("RJCP", LogLevel.Debug)
                .AddNUnitLogger();
            }); 
          }
        }
      }
    }
  }

  [Test]
  public void LogSample1() {
    ILogger logger = LoggerFactory.CreateLogger("RJCP.CodeQuality.NUnitExtensions");
    logger.LogDebug("Debug Message");
    logger.LogTrace("Trace Message");
    logger.LogInformation("Info Message");
    logger.LogWarning("Warning Message");
    logger.LogError("Error Message");
    logger.LogCritical("Critical Message");
  }

  [Test]
  public void LogSample2() {
    ILogger logger = LoggerFactory.CreateLogger("RJCP.CodeQuality.NUnitExtensions");
    logger.LogDebug("Debug Message 2");
    logger.LogTrace("Trace Message 2");
    logger.LogInformation("Info Message 2");
    logger.LogWarning("Warning Message 2");
    logger.LogError("Error Message 2");
    logger.LogCritical("Critical Message 2");
  }
}
```

If the `AddNUnitLogger` was replaced with `AddConsole`, only the first run test
case would log, following test cases would not log. Further, depending on time,
logging from one test case may be appended to the logging from a previous run
test case.

This implementation allows to fix this problem.

## 6. References

[1] [GitHub Issue #3919](https://github.com/nunit/nunit/issues/3919)

## 7. Release History

### Version 0.x

Quality:

- Add test cases to ensure the library works with NUnit 4.1.0 (and 3.x, 2.7.1)
  (DOTNET-1007)

### 7.1. Version 0.6.1

Features:

- Implement SparseStream for testing and simulating data inputs (DOTNET-872,
  DOTNET-862)
- SimpleStream: Provide a stream that does nothing (DOTNET-873, DOTNET-862,
  DOTNET-934)
- WriteOnlyStream: New class for testing streams that are not readable
  (DOTNET-873, DOTNET-862)
- Stream Extensions: Provide ReadStream that copies into a byte array
  (DOTNET-875)
- ReadLimitStream: Provide a stream reader that reads less data than requested
  (DOTNET-874)

Quality:

- CodeQuality: Add README to NuGet Package (DOTNET-807, DOTNET-932)
- Upgrade from .NET 4.5 to .NET 4.6.2 (DOTNET-827)
- Code cleanup (DOTNET-833)
- Upgrade .NET Standard 2.1 to .NET 6.0 (DOTNET-936, DOTNET-938, DOTNET-942,
  DOTNET-945, DOTNET-959)
- Update to .NET 8.0 (DOTNET-982, DOTNET-983, DOTNET-989, DOTNET-990)
- AccessorBaseBestPractices: Fix minor details (DOTNET-960)

### 7.2. Version 0.6.0

Features:

- Upgrade project for .NET Core (DOTNET-343, DOTNET-344, DOTNET-360)
- NUnitExtensions: Add a NUnitLogger (DOTNET-382)
- NUnitExtensions: Provide a NUnitTraceListener (DOTNET-396)
- AccessorBase: Allow getting and setting properties (DOTNET-754)

Bugfixes:

- Deploy: Be able to deploy an empty file (DOTNET-619)
- TestContextAccessor: Fix pointer to correct delegate for strings
- PrivateObject: Invoke should raise MissingMethodException (DOTNET-755)

Quality:

- PrivateType: Provide description for ArgumentException (DOTNET-360)
- Performance: Mark methods as static (DOTNET-360)
- NUnitExtensions: Ignore ScratchOptions having duplicate meanings for the same
  value 0 (DOTNET-360)
- CodeQuality: Authenticode sign the binaries with signtool.exe (DOTNET-353)
- MSBuild: Enable version of binaries based on GIT commits (DOTNET-375)
- Accessor Best Practices: Clean up (DOTNET-493, DOTNET-494, DOTNET-558)
- Platform: Use .NET Core functionality if available (DOTNET-618)
- CodeQuality: Use the platform code from RJCP.Core.Environment (DOTNET-729)
- Use ExceptionDispatchInfo when raising inner exceptions (DOTNET-784)

### 7.3. Version 0.5.1.1

Quality:

- Enable CLSCompliant

### 7.4. Version 0.5.1

Features:

- ScratchPad: Add new option to create directory only if missing (DOTNET-265)

Bugfixes:

- ScratchPad: Allow the scratchpad to be used twice in the same test case
  (DOTNET-264)

Quality:

- CodeQuality: Correct code documentation (HELIOS-1661)
- CodeQuality: Update README.md to use the correct app.config settings

### 7.5. Version 0.5.0.1

- Update NuGet package information

### 7.6. Version 0.5.0

Features:

- Support NUnit 3.x
- Platform: Expose code to get the platform for test cases that might need it
  (HELIOS-1307)
- RJCP.CodeQuality: Rename from NUnitExtensions (HELIOS-1337)

Bugfixes:

- NUnitExtensions: Look for NUnit when not available in the stack trace
  (HELIOS-1301)
- Deploy: Fix crash when the Work Directory is not an absolute path
  (HELIOS-1311)

Quality:

- NUnit3: Fix test case when --work is given to the runner (HELIOS-1310)
- CoreDump: Remove implementation as it's moved to CrashReporter (HELIOS-1294)
- Accessor: Add documentation for best practices (HELIOS-1333)
- ScratchPad: Strip function parenthesis '()' from end of test name

### 7.7. Version 0.4.3

Quality:

- Clean up benign warnings (HELIOS-1162, HELIOS-1169, HELIOS-1196, HELIOS-1206)

### 7.8. Version 0.4.2

Features:

- Resources: Static class to iterate and print all resource strings
  (HELIOS-1101)

### 7.9. Version 0.4.1

Features:

- ScratchPad: New class to simplify creating temporary directories for testing
  (HELIOS-927)

Quality:

- Deploy: Check if calling method has a class inside a TestFixtureAttribute
  (HELIOS-869)
- NUnitExtensions: Don't rely on current working directory when testing
  (HELIOS-869)
- Allow software to compile on Linux

### 7.10. Version 0.4.0

Features:

- Allow the possibility to create a crash dump for deep debugging (HELIOS-785,
  HELIOS-795)
- Provide TestDirectory and WorkDirectory from NUnit.Framework (HELIOS-865)

Bugfixes:

- DeleteFile/DeleteDirectory: Remove usage of buggy FileSystemWatcher
  (HELIOS-722)

Quality:

- NUnitExtensions: Speed up File and Directory Delete operations (HELIOS-433)

### 7.11. Version 0.3.0

Features:

- Provide `AccessorBase` class for testing internal classes (HELIOS-210,
  HELIOS-212, HELIOS-223, HELIOS-226, HELIOS-232, HELIOS-233, HELIOS-241,
  HELIOS-264, HELIOS-265)
- Support for testing non-public constructors (HELIOS-218)
- PrivateObject and PrivateType now always raise TargetInvocationException
  (HELIOS-255)

Bugfixes:

- NullReferenceException is no longer raised when calling an inexistent method
  on a PrivateObject (HELIOS-213)
- Expand implementation of PrivateObject and PrivateType to be compatible with
  MS implementation (HELIOS-224)
- PrivateObject.Target_set correctly updates the RealType (HELIOS-255)

Quality:

- Refactor PrivateTypes and PrivateObject to remove extra classes (HELIOS-241)
- PrivateObject(object, PrivateType) now allows null for object (HELIOS-255)

### 7.12. Version 0.2.1

- Internal refactoring, no external changes

### 7.13. Version 0.2.0

Features:

- Allow deployment of individual files
- Reimplement `PrivateObject` and `PrivateType`

Bugfixes:

- Ensure directories and files are properly deleted before running test cases
  (HELIOS-70, HELIOS-80)

Quality:

- Make `Deploy.Item()` more efficient (DOTNET-13)
- Interpret backslashes on Linux as as a path separator (HELIOS-47)

### 7.14. Version 0.1.0

- Initial Release for NUnit 2.x
