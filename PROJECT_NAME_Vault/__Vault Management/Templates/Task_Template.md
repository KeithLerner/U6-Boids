---
tags:
  - ProjectManagement/Task
Status: To Do
Assignee: 
Due-Date: YYYY-MM-DD
Hours-Estimated: 
Priority: 
Deliverable-Location: ""
Blocks: []
---
## Status:
```dataviewjs
const pages = dv.pages('#ProjectManagement/Task')
    .filter(p => p.Blocks && 
        p.Blocks.map(b => dv.page(b).file.path == dv.current().file.path).includes(true))
if (pages.length > 0) {
    dv.header(2, '<font color = red>THIS TASK IS CURRENTLY BLOCKED BY THE FOLLOWING TASKS</font>:')
    dv.paragraph(pages.map(p => p.file.name))
}
```
`INPUT[inlineSelect(option(To Do), option(In Progress), option(In Revision), option(Awaiting Review), option(Awaiting Implementation), option(Done), option(Cut), showcase):Status]`
### Asset Location (when applicable):
`INPUT[text(showcase):Deliverable-Location]`
## Description
%% What does this task entail? Think of deliverables! %%

### Recommended Approach
%% What is the general idea behind completing this task? Think of actionable items! %%

## Work Log
```dataviewjs
// Function to get the resolved page path from a link
const getPagePath = (link) => {
    if (!link) return '';
    // If it's already a resolved page object, get its path
    if (link.path) return link.path;
    // If it's a link string, try to resolve it
    try {
        return dv.page(link).file.path;
    } catch {
        return String(link);
    }
};

// Function to calculate hours between two datetime strings
const calculateHours = (startTime, endTime) => {
    if (!startTime || !endTime) return 0;
    
    const start = dv.date(startTime);
    const end = dv.date(endTime);
    
    if (!start || !end) return 0;
    
    const diffMs = end - start;
    return diffMs / (1000 * 60 * 60); // Convert milliseconds to hours
};

// Get all work log entries linked to this task
const workLogs = dv.pages("#ProjectManagement/WorkLogEntry")
    .filter(p => p.task && getPagePath(p.task) === dv.current().file.path);

// Calculate hours per author
const authorHours = {};
const authorBorderColors = {};
const authorBgColors = {};
workLogs.forEach(log => {
    if (!log["Author"] || !log["Time-Start"] || !log["Time-End"]) return

	const authorPage = dv.page(log["Author"]);

	const hours = calculateHours(log["Time-Start"], log["Time-End"]);
	const rgb = authorPage.rgb ?? '199, 199, 199';
	const key = authorPage.file.name;
	authorHours[key] = (authorHours[key] || 0) + hours;
	authorBorderColors[key] = `rgba(${rgb}, 1)`;
	authorBgColors[key] = `rgba(${rgb}, 0.8)`;
});

// Prepare data for the chart
const authors = Object.keys(authorHours);
const hours = authors.map(author => authorHours[author]);

// Create chart configuration
const chartData = {
    type: 'pie',
    data: {
        labels: authors.map(author => `${author} (${authorHours[author].toFixed(1)}h)`),
        datasets: [{
            data: hours,
            backgroundColor: Object.values(authorBgColors),
            borderColor: Object.values(authorBorderColors),
            borderWidth: 1
        }]
    },
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'right',
                labels: {
                    padding: 20,
                    font: {
                        size: 12
                    }
                }
            },
            title: {
                display: true,
                text: 'Hours Invested by Author',
                font: {
                    size: 16
                }
            }
        }
    }
};

// Check if we have data before rendering
if (authors.length > 0) {
    window.renderChart(chartData, this.container);
    
    dv.table(
		[
			"Work Log Entry",
			"Author"
		],
		workLogs.map(p => [
			// Work log entry ID column with link to file
			dv.fileLink(p.file.name),
			
			// Author column (using ?? for null coalescing)
			p.Author ?? "No author set"
		])
	);
}
```

```dataviewjs
// Create button element
const button = dv.el('button', 'Create Work Log Entry');

button.addEventListener('click', async () => {

    // Get current file name and extract task ID
    const currentFile = dv.current().file.name;
    const taskId = currentFile.split('Task_')[1];
    
    // Find existing work logs for this task to determine next index
    const workLogs = app.vault.getMarkdownFiles()
        .filter(f => f.name.startsWith(`WL_${taskId}`))
        .map(f => parseInt(f.name.split('-').pop()))
        .filter(n => !isNaN(n));

    const nextIndex = workLogs.length > 0 ? Math.max(...workLogs) + 1 : 1;

    // Generate new file name
    const newFileName = `WL_${taskId}-${nextIndex}`;
    const targetFolder = "_Project Management/Work Log Entries"; // Specify your desired folder
    const newFilePath = `${targetFolder}/${newFileName}.md`;

    // Get template content
    const templatePath = "__Vault Management/Templates/Work_Log_Entry_Template.md";
    const template = await app.vault.adapter.read(templatePath);

    // Parse template and add/modify metadata
    const templateData = template.split('---\n');
    
    // Create new frontmatter
    const frontmatter = {
        "tags":
            'ProjectManagement/WorkLogEntry',
        "Author": '',
        "Time-Start": 'yyyy-mm-ddT--:--:--',
        "Time-End": 'yyyy-mm-ddT--:--:--',
    };

    // Convert frontmatter to YAML
    const yamlFrontmatter = '---\n' + 
        Object.entries(frontmatter)
            .map(([key, value]) => `${key}: ${value}`)
            .join('\n') + 
        '\n' + 
        `task: "[[Task_${taskId}]]"` +
        '\n---\n';

    // Combine everything
    const content = yamlFrontmatter + templateData[2];
    

    // Create new file
    await app.vault.create(newFilePath, content);

    // Open the new file
    const newFile = app.vault.getAbstractFileByPath(newFilePath);
    await app.workspace.activeLeaf.openFile(newFile);
    
});
```
## Notes