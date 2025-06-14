## 🎨 Description 🎨

Imagine this: you are an artist in Venice, Italy and overwhelmed with clients eager to have their portraits painted on a serene gondola ride, you programmed a little robot friend, Mosaic Droid, to assist you in bringing your pixel-art visions to life. 

Mosaic Droid is an application that interprets a specialized programming language, enabling it to paint pixels onto a digital canvas. 

This application allows you to write commands in a text editor or load them from `.pw` files. Mosaic Droid will then execute these commands sequentially, drawing lines, circles, and rectangles, changing brush colors and sizes, and even filling areas with color. You'll manage variables, utilize functions, and implement conditional jumps to control the flow of your artistic program.

**Key functionalities include:**

* **Drawing Primitives:** Commands like `DrawLine`, `DrawCircle`, and `DrawRectangle` allow Mosaic Droid to create fundamental shapes on the canvas.
* **Color and Size Control:** Adjust Wall-E's brush with `Color` and `Size` commands to achieve different artistic effects.
* **Fill Functionality:** The `Fill()` command intelligently paints contiguous areas of the same color.
* **Variable Assignment:** Assign numerical or boolean values to variables for dynamic program control.
* **Built-in Functions:** Utilize functions like `GetActualX()`, `GetActualY()`, `GetCanvasSize()`, `GetColorCount()`, `IsBrushColor()`, `IsBrushSize()`, and `IsCanvasColor()` to retrieve information and make decisions within your code.
* **Conditional Jumps:** Implement `GoTo` statements with labels and conditions to create loops and branching logic, allowing for complex artistic patterns.

## 🤖 Features 🤖

* **User-Friendly Interface (WPF):** A modern graphical interface built with WPF, featuring a text editor with line numbers, a dynamic canvas, and intuitive controls.
* **Language Localization (.NET Resources):** Seamlessly switch the application's language between English and Spanish for a fully customizable experience.
* **Background Music with Mute/Unmute:** Enjoy an immersive experience with optional background music, controllable via dedicated mute/unmute buttons.
* **Information Panel:** A helpful panel on the right side of the interface provides a quick guide to all available commands and their syntax, aiding users in programming Wall-E.
* **Canvas Customization:** Easily adjust the canvas dimensions to your preference, with a button to resize.
* **Code Execution and Error Handling:** Execute your pixel art code with a dedicated button. The application will smartly report syntactic and semantic errors, and gracefully capture and report runtime errors, stopping execution while preserving the painted progress.
* **File Management:** Load existing `.pw` files into the editor and save your current code to `.pw` files for later use.

## 📜 How to Use 📜

1.  **Launch the application:** Run the executable file.
2.  **Select your language:** Choose between English and Spanish using the language switching option.
3.  **Adjust Canvas Dimensions:** Input your desired canvas size (e.g., 256 for a 256x256 pixel canvas) and click the "Redimensionar Canvas" (Resize Canvas) button.
4.  **Write Your Code:** Use the text editor on the left to write your commands.Remember, every valid code must start with `Spawn(x,y)`.
5.  **Execute Your Art:** Click the "Ejecutar Código" (Execute Code) button to see Mosaic Droid bring your pixel art to life on the canvas.
6.  **Load/Save Projects:** Use the "Cargar Archivo" (Load File) and "Guardar Archivo" (Save File) buttons to manage your `.pw` project files.
7.  **Control Music:** Use the music control buttons to mute or unmute the background music.
8.  **Refer to the Information Panel:** If you need a reminder of the language commands, consult the information panel on the right side of the interface.

## 📸 Pictures 📸

* **Main Menu:**
![Game Screenshot](Assets\paisajess.png)

* **Example Code Execution:**
    * *Imagine a screenshot here showing a piece of code in the editor and the corresponding pixel art drawn on the canvas.*

* **Error Reporting:**
    * *Imagine a screenshot here showing an error message displayed for invalid code.*

* **Language Switch:**
    * *Imagine a screenshot showing the UI in Spanish.*

## 🪄 Initial Setup 🪄

1.  **Clone this repository:** `git clone [https://github.com/MenwaLab/Project2PixelWallE]`
2.  **Install .NET SDK:** If not already installed, download and install .NET 9.0 from the official Microsoft website.
3.  **Restore NuGet Packages:** Navigate to the project directory in your terminal and run `dotnet restore` to install all necessary dependencies, including WPF-related packages and .NET Resource Management.
4.  **Open the project:** Open the `.sln` file in your preferred IDE (e.g., Visual Studio).
5.  **Run the project:** Build and run the project from your IDE.

## 🔧 Built With 🔧

- C#
- WPF (Windows Presentation Foundation)
- .NET Resource Management

## 🙏 Recognition 🙏

Special thanks to my professors and the teacher assistants at The University of Havana for their unwavering support. This project, "Pixel Wall-E," represents my second programming project in the Computer Science major, offering an interesting dive into programming concepts and UI frameworks.

## 📩 Contact Info 📩

- Author: Meylí
- Email: [meylijv@gmail.com]
- GitHub: [[https://github.com/MenwaLab](https://github.com/MenwaLab)]
