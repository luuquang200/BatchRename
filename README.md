# Batch Rename Project

This WPF application facilitates bulk file renaming with customizable rules, leveraging several design patterns such as Singleton, Factory, Abstract Factory, and Prototype. The application implements a plugin architecture and utilizes delegate & event functionalities.

## Technical Details

- Design patterns: Singleton, Factory, Abstract factory, Prototype
- Plugin architecture

## Features Implemented

- **Dynamically load renaming rules:** The application can load various renaming rules from external DLL files.
- **Select files and folders:** Users can select all files and folders they want to rename.
- **Create a set of rules:** Users can create a set of rules for renaming files, adding/editing each rule's parameters.
- **Apply rules in numerical order:** The set of rules is applied in numerical order to each file, generating new names.
- **Save rules as presets:** Users can save sets of rules as presets for quick access in the future.
- **Store rule parameters as presets:** The parameters of rules are stored in the form of presets.
- **Drag & Drop files:** Users can drag and drop files to add them to the list.
- **Recursive functionality:** Specify a directory, and the application automatically scans and includes all files within.
- **Exception handling for rule editing:** Ensures character restrictions in file names and prevents exceeding the maximum length of 255 characters.
- **Preview feature:** Allows users to preview the results before renaming.
- **Create file copies and move them to a selected directory:** The application creates copies of all files and moves them to a chosen directory instead of renaming the original files.

## Renaming Rules

The application supports various renaming rules:

1. Change the extension to another extension.
2. Add a counter to the end of the file name.
3. Remove leading and trailing spaces from the filename.
4. Replace specific characters with other characters.
5. Add a prefix to all files.
6. Add a suffix to all files.
7. Convert filenames to lowercase and remove spaces.
8. Convert filenames to PascalCase.

## Demo

Check out the video demo of the project [here](https://youtu.be/gDCHi2yq-P8).

Feel free to explore the application and its functionalities. For usage guidelines and specifics on how to run the project, refer to the documentation or user manual.

For any issues or feedback, please open an issue in the repository.

Happy Renaming!
