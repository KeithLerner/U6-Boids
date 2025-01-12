Documentation for this project is managed as a knowledge vault with [Obsidian](https://obsidian.md/download). This vault presents an exciting opportunity to better document and share the information that the development team accumulates.

As an Obsidian vault, these notes are maintained as markdown files. To learn more about using Obsidian's markdown flavor see the [[ObsidianCheatSheet|Obsidian Cheat Sheet]]. To learn more about the Obsidian software see the official [Obsidian Help site](https://help.obsidian.md/Home)

In order to assure this system functions smoothly, guiding principles have been established.

### Guiding Principles
#### Vault Organization
Files should be organized into folders according to their primary purpose. The main folder purposes are `__Vault Management` and `_Project Management`. There is another folder for reference images or files, named `Resources`, but additional primary folders such as these will depend on the project being developed. Sub folders can be made as needed for better organization.

#### Note Structure
Notes will be named with the shortest readable summary of their purpose. Note names may and should include spaces when applicable, this allows easier navigation using the native Obsidian file explorer. Note names **may not** exceed 60 characters.

##### Meta Data
Note files will start with a meta data section. To add meta data to a note, type `---` on line 1 of the file. This method of adding meta data only works within the obsidian editor. External to the Obsidian application, meta data can be added by following standard YAML practices. Learn more about YAML [here](https://notes.nicolevanderhoeven.com/obsidian-playbook/Using+Obsidian/03+Linking+and+organizing/YAML+Frontmatter).

###### Tags
Every note will receive a tag. Some important example tags include: #DoNotGraph , #ProjectManagement , #Documentation.  Tags help categorize notes further than folder structures allow. Additional sub-tags can be created to increase specificity. Tags should rarely be used in the middle of a document as shown above. Tags must exist in some file somewhere in the vault to search by them, the above acts as an anchor for them within the vault so critical tags aren't lost. Other tags can be added to the above list if needed.

###### Aliases
Notes can optionally be equipped with an alias. Aliases act as secondary file names to reference in searches. Aliases also provide automatic improvements for internal link names. This ReadMe.md file is equipped with the alias "Home".

##### Headers
Headers 2-6 will be used and are indicated by the number of `#` symbols that appear before any line. Use as few headers as possible to convey distinct ideas separately. **Header 1 should not be used**, H1 is often rendered differently depending on how the file is viewed.

##### Footnotes
Footnotes should be used sparingly, normal linking is preferred.

#### Linking and Connections
Obsidian is capable of linking files to each other. This is useful for faster navigation from note to note, and also provides visual feedback in the form of the graph view. Links should be used to relate systems to each other.

> For example, there should be links connecting the flight gun to the personal UI because the flight gun is a part of the personal UI's inventory system. The personal UI's inventory system would be linked to both generic

To force a link to render its contents, add `!` in front of the brackets. Forced rendering does not always work depending on the file type and tends to not work on external links.

> For example, linking to this ReadMe with a rendered preview could look like the following: `![[ReadMe]]`.

##### Internal Links
Internal links are denoted by double brackets (`[[]]`) and should only be used to reference files within this Obsidian vault or the associated unity project hierarchy. There is no guarantee that links to other locations on your machine will stay valid on other machines when collaborating with Git.

> For example, `[[ReadMe]].`

##### External Links
External links are denoted by single brackets (`[]`) and can reference any external internet address. External links have an icon at the end of their text to indicate their external status.

> For example, `[https://obsidian.md/]`.

##### Link Aliases
Links can be given aliases for greater readability. Link aliases should be provided for any link that does not read as grammatically correct English. 

###### Internal Links
Internal link aliases are provided as an argument within brackets denoting addresses. Following the link's address, add `|` followed by the link's alias. 

> For example, linking to the obsidian cheat sheet could look like the following: `[[ObsidianCheatSheet|Obsidian Cheat Sheet]]`.

###### External Links
External links with aliases use different syntax than normal external links. The Alias is provided inside of single brackets (`[]`) immediately followed by single parenthesis (`()`) containing the link address.

> For example, linking to the obsidian website could look like the following: `[Obsidian home page](https://obsidian.md/)`.

#### Graph View
The graph view visualizes connections between notes using links. Circles represent notes as nodes with links as directional edges. Edge directional arrows can be turned on or off. 

> Learn more about graph view [here](https://help.obsidian.md/Plugins/Graph+view).

##### Local Graph View
Local graphs only show connections from the active note and can only be opened using the command `Open local graph`.

### Commands and Hotkeys
Obsidian has a command palette that can be opened with `Ctrl + P` by default. Common hotkeys can be found in this vault's [[ObsidianCheatSheet#Shortcuts ⌨️|cheat sheet]] or [online here](https://forum.obsidian.md/t/obsidian-hotkeys-favorites-and-best-practices/12125).

### Plugins
Add plugins as needed when they improve the documentation process

#### Advanced Tables
Allows for easier creation and management of markdown tables. To use this plugin, create a table normally or use an advanced table command from the command palette. 

#### Dictionary
A standard dictionary for manual spell corrections. We're programmers, sometimes we forget how to spell, our documentation does not need to reflect that.

#### File Hider
File hider is used to hide the unity project from Obsidian's file navigator. This choice was made because Obsidian does not recognize c# files inside the vault by default. To change the visibility of the unity project folder, open the settings modal, navigate to the File Hider page in the left scroll view, then set `Hidden File Visibility` to the desired state.

#### Linting
The Wikipedia page for [Lint (software)](https://en.wikipedia.org/wiki/Lint_%28software%29) describes linting as a program that analyses code to flag errors, bugs, stylistic errors, and suspicious constructs. This vault has a linting plugin called [Linter](https://github.com/platers/obsidian-linter) that focuses on correcting stylistic inconsistencies. This plugin has been set up to lint files whenever they are saved, saving a file can be done via the command palette or with the hotkey `Ctrl + S`. Please save/lint your files often to adhere to the principles of the vault.

##### Linter Spell Correction
Linter spell correction support should *always* remain off to avoid overwriting file names within the vault documentation.