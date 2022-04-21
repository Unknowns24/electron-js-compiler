# electron-js-compiler
This project is oriented to javascript file protection on electron apps by combine, obfuscate and compile js files to one jsc file. 
### How it works
electron-js-compiler is a console application that uses ``bytenode`` for the compilation of the bundle js file. This is posible beacause electron use their own version of v8 engine. If you have more than one js file, the app will combine all js files in just one then the app will compile the bundle file on a jsc file. If you need to ignore a specific file you can do it by adding the file with the full path on a file called `.unkignore` this file needs to be on the root folder of the project.
### Dependencies
- Bytenode as global `npm install -g bytenode`
- Bytenode in your project `npm install --save bytenode`
- Electron as global `npm install -g electron`
- Have the same version of electron globally and in your project package.json
### Limitations
To prevent errors on application bundle creating your code needs to follow this requirements:

- Don't require a local module on a view or html (Intead use ipcRender events) except if the file is specified on the `.unkignore` file

- Don't call a module same as a view (I suggest add "Page" at the final of the view name)

- Don't call a module as Config, this word is reserved for a json files for config, so if you need a config in your app use a json called `config.json` and required it on your index js file

- Path in your local modules needs to be compatible with index location

### License
This project is under a [MIT](https://github.com/Unknowns24/electron-js-compiler/blob/main/LICENSE) license