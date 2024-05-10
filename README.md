# ScreenYu

ScreenShot tool by me

## Requirement
- Windows 7 or later
- .NET 7.0

## Feature
- Global hotkey to make a screenshot and copy to clipboard
- Select the area of screenshot
- Draw lines and rectangles on screenshot

## Key Binding
- Start screenshot: `Ctrl` + `Alt` + `A`
- Reset selection and drawings: `R`
- Selection Mode
	- Initial selection: `Left Mouse Down` -> `Mouse Move` -> `Left Mouse Up`
	- Select fullscreen: `Left Mouse Double-Click`
	- Cancel screenshot: `Esc` 
	- Copy the selection to clipboard: `Right Mouse Click`
	- Enter selection mode: (Default) or `S`
- Drawing Mode
	- Rectangle drawing mode: `F`
	- Line drawing mode: `C`
		- Toggle horizontal or vertical straight lines: Hold `Shift`
	- Change stroke color: `0` to `9`
	- Change stroke size: `Mouse Wheel Up` to increase, `Mouse Wheel Down` to decrease
	- Undo the most recent drawing: `Ctrl` + `Z`

## How to Develop
- Implement `Handler.I` and add it to `HandlerManager`
	- Optionally implement `App.IScreenShotEventHandler` and add it to `ScreenShotStarted`
	- Optionally implement `Handler.Common.IResetHandler`
