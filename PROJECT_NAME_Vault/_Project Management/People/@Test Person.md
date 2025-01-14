---
tags:
  - ProjectManagement/People
rgb: 0, 255, 255
role: DEMO
portfolio: https://obsidian.md/
picture: "[[DemoPerson.png]]"
---
## Personal Goals
%% Ordered list of professional priorities (1 is highest priority) during the duration of this project. %%
1. Test the vault to make sure every PM feature works correctly
2. Get 12 strikes and get kicked off the project
3. Accumulate maximum story points
## Time Invested
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
    
    const start = dv.date(startTime).toMillis();
    const end = dv.date(endTime).toMillis();
    
    if (!start || !end) return 0;
    
    const diffMs = end - start;
    return diffMs / (1000 * 60 * 60); // Convert milliseconds to hours
};

// Function to calculate total time invested from work logs
const calculateTimeInvested = (page) => {
    // Get all linked pages
    const links = page.file.outlinks;
    if (!links || !links.length) return 0;
    
    // Calculate total hours from all work logs
    let totalHours = 0;
    
    links.forEach(link => {
        try {
            const workLog = dv.page(link);
            if (workLog && workLog["Time-Start"] && workLog["Time-End"]) {
                totalHours += calculateHours(workLog["Time-Start"], workLog["Time-End"]);
            }
        } catch (e) {
            console.error(`Error processing work log ${link}:`, e);
        }
    });
    
    return totalHours.toFixed(2); // Return with 2 decimal places
};

// Get work log entry pages where author is current person
const pages = dv.pages('#ProjectManagement/WorkLogEntry')
    .filter(page =>
        page.Author &&
        getPagePath(page.Author) == getPagePath(dv.current().file.path))

// Data to update
const trackerData = {
    entries: [],
    separateMonths: true,
    heatmapSubtitle: "This visualizes what days someone tends to work to allow for easier scheduling of meetings.",
    
}

// You need dataviewjs plugin to get information from your pages
for(let page of pages){
    trackerData.entries.push({
        date: dv.date(page["Time-Start"]),
        intensity: calculateTimeInvested(page),
        content: await dv.span(`[](${page.file.name})`)
    });
}
  
renderHeatmapTracker(this.container, trackerData);
```
## Tasks
### Assigned Task Breakdown
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

// Get current file path
const currentFilePath = dv.current().file.path;

// Get all pages with the tag
const pages = dv.pages('#ProjectManagement/Task')
        .filter(page =>
            Array.isArray(page.Assignee) &&
            page.Assignee.map(a => {
                return getPagePath(a) == getPagePath(dv.current().file.path)
            }).includes(true)
        )

// Get counts for each status
const statusCounts = {
    "To Do": pages.filter(p => p["Status"] === "To Do").length,
    "To Revise": pages.filter(p => p["Status"] === "To Revise").length,
    "In Progress": pages.filter(p => p["Status"] === "In Progress").length,
    "In Revision": pages.filter(p => p["Status"] === "In Revision").length,
    "Awaiting Review": pages.filter(p => p["Status"] === "Awaiting Review").length,
    "Awaiting Implementation": pages.filter(p => p["Status"] === "Awaiting Implementation").length,
    "Done": pages.filter(p => p["Status"] === "Done").length
};

// Define status labels in desired order
const labels = [
    "To Do",
    "To Revise",
    "In Progress",
    "In Revision",
    "Awaiting Review",
    "Awaiting Implementation",
    "Done"
];

// Define colors for different statuses
const colors = {
    "To Do": 'rgba(255, 99, 132, 0.2)',
    "To Revise": 'rgba(255, 159, 64, 0.2)',
    "In Progress": 'rgba(255, 205, 86, 0.2)',
    "In Revision": 'rgba(75, 192, 192, 0.2)',
    "Awaiting Review": 'rgba(54, 162, 235, 0.2)',
    "Awaiting Implementation": 'rgba(153, 102, 255, 0.2)',
    "Done": 'rgba(99, 255, 132, 0.2)'
};

const borderColors = {
    "To Do": 'rgba(255, 99, 132, 1)',
    "To Revise": 'rgba(255, 159, 64, 1)',
    "In Progress": 'rgba(255, 205, 86, 1)',
    "In Revision": 'rgba(75, 192, 192, 1)',
    "Awaiting Review": 'rgba(54, 162, 235, 1)',
    "Awaiting Implementation": 'rgba(153, 102, 255, 1)',
    "Done": 'rgba(99, 255, 132, 1)'
};

const chartData = {
    type: 'bar',
    data: {
        labels: labels,
        datasets: [{
            label: 'Number of Tasks',
            data: labels.map(label => statusCounts[label]),
            backgroundColor: labels.map(label => colors[label]),
            borderColor: labels.map(label => borderColors[label]),
            borderWidth: 1,
            borderJoinStyle: 'round',
            borderRadius: 8,
        }]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true,
                ticks: {
                    stepSize: 1
                }
            }
        },
        plugins: {
            legend: {
                display: false  // Hide legend since colors are self-explanatory
            }
        }
    }
};

window.renderChart(chartData, this.container);
```
### Overdue
```dataviewjs
// Define the folder path
const folderPath = "_Project Management/Tasks";

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
    
    const start = dv.date(startTime).toMillis();
    const end = dv.date(endTime).toMillis();
    
    if (!start || !end) return 0;
    
    const diffMs = end - start;
    return diffMs / (1000 * 60 * 60); // Convert milliseconds to hours
};

// Function to calculate total time invested from work logs
const calculateTimeInvested = (page) => {
    // Get all linked pages
    const links = page.file.outlinks;
    if (!links || !links.length) return 0;
    
    // Calculate total hours from all work logs
    let totalHours = 0;
    
    links.forEach(link => {
        try {
            const workLog = dv.page(link);
            if (workLog && workLog["Time-Start"] && workLog["Time-End"]) {
                totalHours += calculateHours(workLog["Time-Start"], workLog["Time-End"]);
            }
        } catch (e) {
            console.error(`Error processing work log ${link}:`, e);
        }
    });
    
    return totalHours.toFixed(2); // Return with 2 decimal places
};

// Get current file path
const currentFilePath = dv.current().file.path;

// Create the table with filtered results
dv.table(
    [
        "Task ID",
        "Due Date",
        "Description",
        "Hours Estimate",
        "Hours Invested"
    ],
    dv.pages(`"${folderPath}"`)
        .filter(page => page.Status && page.Status != 'Done')
        .filter(page =>
            Array.isArray(page.Assignee) &&
            page.Assignee.map(a => {
                return getPagePath(a) == getPagePath(dv.current().file.path)
            }).includes(true)
        )
        .filter(page => page["Due-Date"] && dv.date(page["Due-Date"]) < dv.date("today"))
        .map(page => [
            // Task ID column with link to file
            dv.fileLink(page.file.name),
            
            // Due Date column (using ?? for null coalescing)
            page["Due-Date"] ? dv.date(page["Due-Date"]) : "No date set",
            
            // Description column - get content under Description header
            (page.file.tasks
                .find(t => t.header?.subpath === "Description")
                ?.text) ?? "No description",
            
            // Hours Estimate column
            page["Hours-Estimated"] ?? "Not estimated",
            
            // Hours Invested column - calculated from work logs
            `${calculateTimeInvested(page)} hours`
        ])
)
```
### Due Today
```dataviewjs
// Define the folder path
const folderPath = "_Project Management/Tasks";

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
    
    const start = dv.date(startTime).toMillis();
    const end = dv.date(endTime).toMillis();
    
    if (!start || !end) return 0;
    
    const diffMs = end - start;
    return diffMs / (1000 * 60 * 60); // Convert milliseconds to hours
};

// Function to calculate total time invested from work logs
const calculateTimeInvested = (page) => {
    // Get all linked pages
    const links = page.file.outlinks;
    if (!links || !links.length) return 0;
    
    // Calculate total hours from all work logs
    let totalHours = 0;
    
    links.forEach(link => {
        try {
            const workLog = dv.page(link);
            if (workLog && workLog["Time-Start"] && workLog["Time-End"]) {
                totalHours += calculateHours(workLog["Time-Start"], workLog["Time-End"]);
            }
        } catch (e) {
            console.error(`Error processing work log ${link}:`, e);
        }
    });
    
    return totalHours.toFixed(2); // Return with 2 decimal places
};

// Get current file path
const currentFilePath = dv.current().file.path;

// Create the table with filtered results
dv.table(
    [
        "Task ID",
        "Due Date",
        "Description",
        "Hours Estimate",
        "Hours Invested"
    ],
    dv.pages(`"${folderPath}"`)
        // Filter for tasks assigned to current user, comparing resolved page paths
        .filter(page => page.Status && page.Status != 'Done')
        .filter(page =>
            Array.isArray(page.Assignee) &&
            page.Assignee.map(a => {
                return getPagePath(a) == getPagePath(dv.current().file.path)
            }).includes(true)
        )
        .filter(page => page["Due-Date"] && dv.date(page["Due-Date"]) == dv.date("today"))
        .map(page => [
            // Task ID column with link to file
            dv.fileLink(page.file.name),
            
            // Due Date column (using ?? for null coalescing)
            page["Due-Date"] ? dv.date(page["Due-Date"]) : "No date set",
            
            // Description column - get content under Description header
            (page.file.tasks
                .find(t => t.header?.subpath === "Description")
                ?.text) ?? "No description",
            
            // Hours Estimate column
            page["Hours-Estimated"] ?? "Not estimated",
            
            // Hours Invested column - calculated from work logs
            `${calculateTimeInvested(page)} hours`
        ])
)
```
### Upcoming
```dataviewjs
// Define the folder path
const folderPath = "_Project Management/Tasks";

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
    
    const start = dv.date(startTime).toMillis();
    const end = dv.date(endTime).toMillis();
    
    if (!start || !end) return 0;
    
    const diffMs = end - start;
    return diffMs / (1000 * 60 * 60); // Convert milliseconds to hours
};

// Function to calculate total time invested from work logs
const calculateTimeInvested = (page) => {
    // Get all linked pages
    const links = page.file.outlinks;
    if (!links || !links.length) return 0;
    
    // Calculate total hours from all work logs
    let totalHours = 0;
    
    links.forEach(link => {
        try {
            const workLog = dv.page(link);
            if (workLog && workLog["Time-Start"] && workLog["Time-End"]) {
                totalHours += calculateHours(workLog["Time-Start"], workLog["Time-End"]);
            }
        } catch (e) {
            console.error(`Error processing work log ${link}:`, e);
        }
    });
    
    return totalHours.toFixed(2); // Return with 2 decimal places
};

// Get current file path
const currentFilePath = dv.current().file.path;

// Create the table with filtered results
dv.table(
    [
        "Task ID",
        "Due Date",
        "Description",
        "Hours Estimate",
        "Hours Invested"
    ],
    dv.pages(`"${folderPath}"`)
        .filter(page => page.Status && page.Status != 'Done')
        .filter(page =>
            Array.isArray(page.Assignee) &&
            page.Assignee.map(a => {
                return getPagePath(a) == getPagePath(dv.current().file.path)
            }).includes(true)
        )
        .filter(page => page["Due-Date"] && dv.date(page["Due-Date"]) > dv.date("today"))
        .map(page => [
            // Task ID column with link to file
            dv.fileLink(page.file.name),
            
            // Due Date column (using ?? for null coalescing)
            page["Due-Date"] ? dv.date(page["Due-Date"]) : "No date set",
            
            // Description column - get content under Description header
            (page.file.tasks
                .find(t => t.header?.subpath === "Description")
                ?.text) ?? "No description",
            
            // Hours Estimate column
            page["Hours-Estimated"] ?? "Not estimated",
            
            // Hours Invested column - calculated from work logs
            `${calculateTimeInvested(page)} hours`
        ])
)
```
### Done
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
    
    const start = dv.date(startTime).toMillis();
    const end = dv.date(endTime).toMillis();
    
    if (!start || !end) return 0;
    
    const diffMs = end - start;
    return diffMs / (1000 * 60 * 60); // Convert milliseconds to hours
};

// Function to calculate total time invested from work logs
const calculateTimeInvested = (page) => {
    // Get all linked pages
    const links = page.file.outlinks;
    if (!links || !links.length) return 0;
    
    // Calculate total hours from all work logs
    let totalHours = 0;
    
    links.forEach(link => {
        try {
            const workLog = dv.page(link);
            if (workLog && workLog["Time-Start"] && workLog["Time-End"]) {
                totalHours += calculateHours(workLog["Time-Start"], workLog["Time-End"]);
            }
        } catch (e) {
            console.error(`Error processing work log ${link}:`, e);
        }
    });
    
    return totalHours.toFixed(2); // Return with 2 decimal places
};

// Get current file path
const currentFilePath = dv.current().file.path;

// Get pages
const pages = dv.pages('#ProjectManagement/Task')
    .filter(page => page.Status && page.Status == 'Done')
    .filter(page =>
        Array.isArray(page.Assignee) &&
        page.Assignee.map(a => {
            return getPagePath(a) == getPagePath(dv.current().file.path)
        }).includes(true)
    )

// Create the table with filtered results
dv.table(
    [
        "Task ID",
        "Due Date",
        "Description",
        "Hours Estimate",
        "Hours Invested"
    ],
    pages.map(page => [
            // Task ID column with link to file
            dv.fileLink(page.file.name),
            
            // Due Date column (using ?? for null coalescing)
            page["Due-Date"] ? dv.date(page["Due-Date"]) : "No date set",
            
            // Description column - get content under Description header
            (page.file.tasks
                .find(t => t.header?.subpath === "Description")
                ?.text) ?? "No description",
            
            // Hours Estimate column
            page["Hours-Estimated"] ?? "Not estimated",
            
            // Hours Invested column - calculated from work logs
            `${calculateTimeInvested(page)} hours`
        ])
)
```
## Recent Mentions
```dataviewjs
const file = dv.current().file;
const fileName = file.name;
const filePath = file.path;

async function getMentions(f) {
	const content = await dv.io.load(f.path);
	const mentions = [];
	
	// Split into frontmatter and content
        const parts = content.split('---\n');
        let mainContent;
        
        if (parts.length >= 3) {
            mainContent = parts.slice(2).join('---\n');
        } else {
            mainContent = content;
        }
        
        const lines = mainContent.split('\n');
        
        for (let i = 0; i < lines.length; i++) {
            const line = lines[i];
            
            // Check for mentions using the path
            if ((line.includes(`[[${filePath}]]`) || 
	            line.includes(`](${filePath})`) ||
	            line.includes(filePath) ||
	            line.includes(`[[${fileName}]]`) ||
	            line.includes(`](${fileName})`) ||
	            line.includes(fileName)) &&
	            (!line.includes("Assignee: ") &&
	            !line.includes("Author: "))) {
                // Get context
                const contextStart = Math.max(0, i - 1);
                const contextEnd = Math.min(lines.length - 1, i + 1);
                let context = [];
                
                for (let j = contextStart; j <= contextEnd; j++) {
                    let contextLine = lines[j].trim();
                    if (j === i) {
                        contextLine = `**${contextLine}**`;
                    }
                    if (contextLine) {
                        context.push(contextLine);
                    }
                }
                
                mentions.push(context.join('\n'));
            }
        }
        
        return mentions;
}

// Get linked pages and process them
const linkedPages = dv.pages()
    .filter(p => p.file.outlinks.some(l => l.path === filePath))
    .sort(p => p.file.mtime, 'desc')
    .slice(0, 20);
    
for(i = 0; i < linkedPages.length; i++) {
	const page = linkedPages[i];
	const mentions = await getMentions(page.file);
	if (mentions.length > 0) {
		dv.header(2, dv.fileLink(linkedPages[i].file.path, false, linkedPages[i].file.name));
		dv.list(mentions);
		dv.span("---");
	}
}
```
