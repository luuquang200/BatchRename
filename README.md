# Batch Rename Project

## Technical Details

- Design Patterns: Singleton, Factory, Abstract Factory, Prototype
- Plugin Architecture

## Completed Functionalities

This project implements the following functionalities:

- **Dynamically Load Renaming Rules**: External DLL files can be loaded to obtain various renaming rules.
- **Select Files and Folders**: Users can select the files and folders they want to rename.
- **Create a Set of Rules**: A menu allows users to create a set of rules for file renaming, each rule being editable.
- **Apply Sequential Rules**: The set of rules is applied in numerical order to each file, giving them a new name.
- **Save Rule Sets as Presets**: Users can save these sets of rules for quick access when needed.
- **Store Rule Parameters as a Preset**: Parameters of the rules are stored as presets.
- **Drag and Drop Files**: Capability to drag and drop files to add them to the list.
- **Recursive Functionality**: Specify a directory, and the application automatically scans and adds all files within.
- **Exception Handling in Rule Editing**: Checks for characters not allowed in file names and ensures file name length doesn't exceed 255 characters.
- **Preview Before Applying Changes**: Users can preview the changes before applying them.
- **Create File Backups**: Copies of all files are made and moved to a selected directory rather than renaming the original files.

Feel free to explore the application and its functionalities. For usage guidelines and specifics on how to run the project, refer to the documentation or user manual.

For any issues or feedback, please open an issue in the repository.

Happy Renaming!
