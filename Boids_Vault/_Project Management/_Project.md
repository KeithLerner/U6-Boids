---
tags:
  - ProjectManagement
Project-Start: 2024-12-01T06:00:00
Project-End: 2025-05-01T23:59:00
UnityEngineVersion: 6000.0.33f1
---
## Meet the Team
```dataviewjs
const pages = dv.pages('#ProjectManagement/People')
	.filter(page => page.file.name != '@Test Person' && page.file.name != 'People_Template')
	.sort(function(a,b){return a.file.role - b})

pages.forEach(page => {
	if (page.picture) {
		dv.header(3, dv.fileLink(page.file.name) + ": " + page.role);
		dv.paragraph(page.portfolio)
		dv.paragraph('!' + page.picture);
	}
})
```
## Project Details
> Check out the [[MASTER DESIGN DOC|Master Design Document (MASTER DOC)]] for more design details

Unity Engine Version: `INPUT[text(showcase):UnityEngineVersion]`

```dataviewjs
dv.header(3, 'Sprints')
dv.list(dv.pages('#ProjectManagement/Sprint')
		.filter(p => !p.file.path.split('/').includes('Templates'))
		.sort()
		.map(p => dv.fileLink(p.file.path))) 
		
```
```dataviewjs
// Create button element
const button = dv.el('button', 'Create New Sprint');

const tasksCreatedSoFar = dv.pages('#ProjectManagement/Sprint')
	.filter(p => !p.file.path.split('/').includes('Templates'))
	.length

button.addEventListener('click', async () => {
    // Generate new file name
    const newFileName = `Sprint_${tasksCreatedSoFar}`;
    const targetFolder = "_Project Management/Sprints"; // Specify your desired folder
    const newFilePath = `${targetFolder}/${newFileName}.md`;

    // Get template content
    const templatePath = "__Vault Management/Templates/Sprint_Template.md";
    const template = await app.vault.adapter.read(templatePath);    

    // Create new file
    await app.vault.create(newFilePath, template);

    // Open the new file
    const newFile = app.vault.getAbstractFileByPath(newFilePath);
    await app.workspace.activeLeaf.openFile(newFile);
});
```
## Work Details
### Sum Hours by Person
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

// Calculate hours per author with daily breakdown
function calculateWorkBreakdown(logs) {
    const endDate = dv.current()["Project-End"];
    const startDate = dv.current()["Project-Start"];
    const dailyWork = {}; // Work time by date by author

    if (startDate) {
        dailyWork[startDate] = {}
        dailyWork[startDate]["@Test Person"] = 0
    }
    
    logs.forEach(log => {
        if (log["Author"] && log["Time-Start"] && log["Time-End"]) {
            // Get author name from page link
            const authorName = dv.page(log["Author"]).file.name;
            
            // Calculate hours for this log
            const hours = calculateHours(log["Time-Start"], log["Time-End"]);
            
            // Convert date to ISO string for consistency
            const dateKey = log["Time-Start"].toISODate();
            
            // Initialize date entry if it doesn't exist
            if (!Object.keys(dailyWork).contains(dateKey)) {
                dailyWork[dateKey] = {};
            }
            
            // Update hours for this author on this date
            if (!dailyWork[dateKey][authorName]) {
                dailyWork[dateKey][authorName] = 0;
            }
            dailyWork[dateKey][authorName] += hours;
        }
    });

    if (endDate) {
        dailyWork[endDate] = {}
        dailyWork[endDate]["@Test Person"] = 0
    }

    return dailyWork;
}

// Get current file path
const currentFilePath = dv.current().file.path;

// Get all work log entries linked to this person
const workLogs = dv.pages("#ProjectManagement/WorkLogEntry")
	.filter(p => p["Time-Start"] && p["Time-End"] && 
	dv.date(p["Time-Start"]) > dv.date(dv.current()["Project-Start"]) && 
	dv.date(p["Time-End"]) <= dv.date(dv.current()["Project-End"]));

// Calculate hours per author
const workBreakdown = calculateWorkBreakdown(workLogs);

// Step 1: Flatten data into an array of objects
let flattenedWorkBreakdown = [];
for (let date in workBreakdown){
    for(let author in workBreakdown[date]) {
        flattenedWorkBreakdown.push({ date: date, author: author, time: workBreakdown[date][author] });
    }
}

// Step 2: Calculate cumulative sums
let cumulativeData = {};
let authors = new Set(flattenedWorkBreakdown.map(entry => entry.author));

authors.forEach(author => {
    cumulativeData[author] = [];
    let cumulativeSum = 0;
    for (let date in workBreakdown) {
        cumulativeSum += workBreakdown[date][author] || 0;
        cumulativeData[author].push({ date: date, cumulativeTime: cumulativeSum });
    }
});

// Step 3: Get author colors
const rgbData = {};
authors.forEach(author => {
	rgbData[author] = dv.page(author).rgb;
});
const color = (rgb) => 'rgba(' + rgb + ', 0.2)';
const borderColor = (rgb) => 'rgba(' + rgb + ', 1)';

// Step 4: Prepare data for chart
let chartData = {
    labels: Object.keys(workBreakdown).sort(),
    datasets: []
};

authors.forEach(author => {
    chartData.datasets.push({
        label: author,
        data: cumulativeData[author].map(entry => entry.cumulativeTime),
        borderColor: borderColor(rgbData[author]),
        tension: 0.3,
        stepped: true,
    });
});

// Step 5: Render the chart
const chart = {
    type: 'line',
    data: chartData,
    options: {
        responsive: true,
        scales: {
            x: {
                type: 'time',
                time: {
                    unit: 'day'
                }
            },
        }
    }
};
window.renderChart(chart, this.container);

// Utility function to generate random colors
function getRandomColor() {
    return 'rgba(' + Math.floor(Math.random() * 256) + ',' + Math.floor(Math.random() * 256) + ',' + Math.floor(Math.random() * 256) + ', 0.5)';
}
```
## Tasks
```dataviewjs
// Create button element
const button = dv.el('button', 'Create New Task');

const tasksCreatedSoFar = dv.pages('#ProjectManagement/Task').length

button.addEventListener('click', async () => {
    // Generate new file name
    const newFileName = `Task_${tasksCreatedSoFar}`;
    const targetFolder = "_Project Management/Tasks"; // Specify your desired folder
    const newFilePath = `${targetFolder}/${newFileName}.md`;

    // Get template content
    const templatePath = "__Vault Management/Templates/Task_Template.md";
    const template = await app.vault.adapter.read(templatePath);    

    // Create new file
    await app.vault.create(newFilePath, template);

    // Open the new file
    const newFile = app.vault.getAbstractFileByPath(newFilePath);
    await app.workspace.activeLeaf.openFile(newFile);
});
```
### Task Breakdown by Sprint
```dataviewjs
const tasks = dv.pages("#ProjectManagement/Task")
	.filter(page => page["Due-Date"] && page["Assignee"]);
const sprints = dv.pages("#ProjectManagement/Sprint")
	.filter(page => page["Sprint-Start"] && page["Sprint-End"])
    .sort(page => page.file.name);

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

// Get data
const sprintNames = []
const sprintStartEnds = [];
sprints.forEach(sprint => {
    const sprintName = sprint.file.name;
    const sprintStart = sprint["Sprint-Start"];
    const sprintEnd = sprint["Sprint-End"];
    const data = {name: sprintName, start: sprintStart, end: sprintEnd};

    sprintNames.push(sprintName);
    sprintStartEnds.push(data);
})

const sprintStatusCounts = [] // array of [task status counts] ordered by sprint
sprintNames.forEach(sprintName => {
    const sprintStartEnd = sprintStartEnds.find((sprintStartEnd, index, array) => { 
        return sprintStartEnd.name == sprintName;
    })
    const start = sprintStartEnd.start;
    const end = sprintStartEnd.end;
    const pages = tasks
        .filter(task => (task["Due-Date"] && 
            dv.date(task["Due-Date"]) > dv.date(start) && 
            dv.date(task["Due-Date"]) <= dv.date(end)))
    const statusCounts = [
        pages.filter(p => p["Status"] === "To Do").length,
        pages.filter(p => p["Status"] === "To Revise").length,
        pages.filter(p => p["Status"] === "In Progress").length,
        pages.filter(p => p["Status"] === "In Revision").length,
        pages.filter(p => p["Status"] === "Awaiting Review").length,
        pages.filter(p => p["Status"] === "Awaiting Implementation").length,
        pages.filter(p => p["Status"] === "Done").length
    ];

    sprintStatusCounts.push(statusCounts)
})

const statusSprintCounts = [[],[],[],[],[],[],[]]; // array of [sprint ordered task count] ordered by status
sprintStatusCounts.forEach(statusCounts => {
    for(let statusCount in statusCounts) {
        statusSprintCounts[statusCount].push(statusCounts[statusCount]);
    }
})

var i = 0;
const datasets = statusSprintCounts.map(sprintCounts => {
    const dataset = {
        label: labels[i++],
        data: sprintCounts,
        backgroundColor: Object.values(colors)[i-1],
        borderColor: Object.values(borderColors)[i-1],
        borderWidth: 1,
        borderJoinStyle: 'round',
        borderRadius: 8,
    }
    return dataset;
})


const chartData = {
    type: 'bar',
    data: {
        labels: sprintNames,
        datasets: datasets
    }
}

window.renderChart(chartData, this.container)
```
