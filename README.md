# Boolean Expression Parser <!-- omit in toc -->

A library and accompanying CLI and WebUI for parsing, evaluating and generating truth tables for boolean logic expressions.


## Contents <!-- omit in toc -->

- [Make up of the project](#make-up-of-the-project)
- [Wiki](#wiki)
- [Found an issue?](#found-an-issue)



## Make up of the project

There are three main parts to the project:

- The library, which is the core of the project. It contains the logic for parsing, evaluating and generating truth tables for boolean expressions.
  - This is the C# project named `BooleanExpressionParser`.
- The CLI, which provides a easy-to-use command line interface for the library.
  - This is the C# project named `BooleanExpressionParser.CLI`.
- The WebUI, which is a Blazor WebAssembly appplication built ontop of an ASP.NET server. It provides a web interface for the library.
  - This is the C# project named `BooleanExpressionParser.Web`.


## Wiki

For more information about usage, project structure, and functionality, please see the [wiki](https://github.com/tomc128/boolean-expression-parser/wiki).


## Found an issue?

If you've found an issue, like an expression that casuses a crash or an incorrectly parsed expression, please open an issue on GitHub. Please include the expression that caused the issue, thank you! Or, if you're able to, fork the repo and fix it yourself. Pull requests are welcome!