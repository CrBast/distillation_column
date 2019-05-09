const electron = require('electron');
const url = require('url');
const path = require('path');

const { app, BrowserWindow, Menu, MenuItem } = electron;

let mainWindow;

const mainMenuTemplate = [
  {
    label: 'File',
    submenu: [
      {
        role: 'reload'
      }
    ]
  }
]

const menu = Menu.buildFromTemplate(mainMenuTemplate)
Menu.setApplicationMenu(menu)


app.on('ready', () => {
  mainWindow = new BrowserWindow({})
  mainWindow.loadURL(url.format({
    pathname: path.join(__dirname, '/view/main.html'),
    protocol: 'file:',
    slashes: true
  }))
})