# stark-lang

<img align="right" width="160px" height="160px" src="img/logo.png">

This is the main repository of the stark programming language.

> The repository is in a **prototyping stage** and is going through multiple phase of prototypes.
>
> A first functional prototype was using a fork of Roslyn and the code is now only available in the branch: [roslyn-fork-prototype](https://github.com/stark-lang/stark/tree/roslyn-fork-prototype)
>
> The next iteration of the prototype will be more disruptive and so can't reuse Roslyn.

## About

You can find more details about this project in the following blog post series:

- [The Odyssey of Stark and Melody](https://xoofx.github.io/blog/2020/03/05/stark-melody-dotnet-sel4/)
- [Stark - Language And Frontend Compiler - Prototype 2019](https://xoofx.github.io/blog/2020/03/06/stark-language-frontend-compiler/)

## IDE Syntax Highlighting

You need to have Visual Studio Code installed.

- Open the folder `src/ide/vscode/stark` with Visual Studio Code
- Run the build task via the menu `Terminal/Run Build Task...` (or `CTRL+SHIFT+B` on Windows).
  - If the build is failing, it could be a misconfiguration of npm/tsc. Open a terminal from VSCode and run the command `npm install -g typescript`. Check that `tsc --version` is running fine (you should have a version above `3.x` or more)
- Press F5 and it should open a new Visual Studio Code instance with the syntax highlighting for Stark and a pseudo integration with a compiler server (nothing is actually plugged there).
- With this new Visual Studio Code instance, open the folder `src/runtime` and you should be able to play with the syntax/language by changing the `core` library.

## License

This software is released under the [BSD-Clause 2 license](http://opensource.org/licenses/BSD-2-Clause).

