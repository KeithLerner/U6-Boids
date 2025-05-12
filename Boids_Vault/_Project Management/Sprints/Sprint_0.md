---
tags:
  - ProjectManagement/Sprint
Sprint-Start: 2025-01-01T00:00:00
Sprint-End: 2025-01-13T23:59:00
---
## Sprint Hours by Day
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
    const authors = [];
    const dailyWork = {};            // Work time by date by author
    
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
                dailyWork[dateKey] = { };
            }

            if (!authors.includes(authorName)) {
                authors.push(authorName);
            }
            
            // Update hours for this author on this date
            if (!dailyWork[dateKey][authorName]) {
                dailyWork[dateKey][authorName] = 0;
            }
            dailyWork[dateKey][authorName] += hours;
        }
    });

    return { dailyWork , authors };
}

// Get current file path
const currentFilePath = dv.current().file.path;

// Get all work log entries linked to this person
const workLogs = dv.pages("#ProjectManagement/WorkLogEntry")
	.filter(p => p["Time-Start"] && p["Time-End"] && 
	dv.date(p["Time-Start"]) > dv.date(dv.current()["Sprint-Start"]) && 
	dv.date(p["Time-End"]) <= dv.date(dv.current()["Sprint-End"]));

// Calculate hours per author
const workBreakdown = calculateWorkBreakdown(workLogs);
const dailyWork = workBreakdown["dailyWork"];
const authors = workBreakdown["authors"];

// Get date range
const endDate = dv.current()["Sprint-End"];
var date = dv.current()["Sprint-Start"];
const dates = [];
while (date.toISODate() < endDate.toISODate()) {
	dates.push(date.toISODate());
	date = date.plus(dv.duration("1 days"));
}

// Access total hours for an author
// workBreakdown.totalHours["John Smith"]

// Access work done on a specific date by an author
// workBreakdown.dailyBreakdown["2025-01-05"].get("John Smith")

// Get all dates
// Object.keys(workBreakdown.dailyBreakdown)

// Get all authors who worked on a specific date
// Array.from(workBreakdown.dailyBreakdown["2025-01-05"].keys())

// Define colors
const rgbData = {};
authors.forEach(author => {
	rgbData[author] = dv.page(author).rgb;
});
const color = (rgb) => 'rgba(' + rgb + ', 0.2)';
const borderColor = (rgb) => 'rgba(' + rgb + ', 1)';

const chartData = {
    type: 'line',
    data: {
        labels: dates,
        datasets: Array.from(authors).map((author) => ({
            label: author,
            data: dates.map(date => dailyWork[date]?.[author] || 0),
            borderColor: [ borderColor(rgbData[author]) ],
            fill: false,
            tension: 0.3
        }))
    },/*
    data: {
        labels: dates,
        datasets: [{
            label: 'Recent Hours Worked',
            data: [ ],//workBreakdown[date][author],
            backgroundColor: color,
            borderColor: borderColor,
            borderWidth: 1
        }]
    },*/
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
## Tasks
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

// Function to format date 
const formatDate = (date) => { 
	if (!date) return null; 
	return date.toFormat("MM-dd-yy"); 
};

// Function to convert date formats
function parseMMDDYY (dateString) { 
	if (!dateString) return null;
	const [month, day, year] = dateString.split("-").map(Number); 
	const fullYear = year < 100 ? 2000 + year : year; // Handle two-digit years 
	return new Date(fullYear, month - 1, day); // Months are 0-based
}

// Get current file path
const currentFilePath = dv.current().file.path;

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

// Get task pages for this sprint
const sprintTaskPages = dv.pages('#ProjectManagement/Task')
        // Filter for tasks with dealines within the current sprint start and end points
        .filter(page => page["Due-Date"] && parseMMDDYY(formatDate(dv.date(page["Due-Date"]))) > parseMMDDYY(formatDate(dv.date(dv.current()["Sprint-Start"]))) && parseMMDDYY(formatDate(dv.date(page["Due-Date"]))) <= parseMMDDYY(formatDate(dv.date(dv.current()["Sprint-End"]))));

// Create the table with filtered results
dv.table(
    Object.keys(colors),
    [
        [sprintTaskPages
            .filter(page => page["Status"] == "To Do")
            .map(page => `[[${page.file.name}]]`),
        sprintTaskPages
            .filter(page => page["Status"] == "To Revise")
            .map(page => `[[${page.file.name}]]`),
        sprintTaskPages
            .filter(page => page["Status"] == "In Progress")
            .map(page => `[[${page.file.name}]]`),
        sprintTaskPages
            .filter(page => page["Status"] == "In Revision")
            .map(page => `[[${page.file.name}]]`),
        sprintTaskPages
            .filter(page => page["Status"] == "Awaiting Review")
            .map(page => `[[${page.file.name}]]`),
        sprintTaskPages
            .filter(page => page["Status"] == "Awaiting Implementation")
            .map(page => `[[${page.file.name}]]`),
        sprintTaskPages
            .filter(page => page["Status"] == "Done")
            .map(page => `[[${page.file.name}]]`)]
    ]
)
```
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
### Progress
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

// Function to format date 
const formatDate = (date) => { 
	if (!date) return null; 
	return date.toFormat("MM-dd-yy"); 
};

// Function to convert date formats
function parseMMDDYY (dateString) { 
	if (!dateString) return null;
	const [month, day, year] = dateString.split("-").map(Number); 
	const fullYear = year < 100 ? 2000 + year : year; // Handle two-digit years 
	return new Date(fullYear, month - 1, day); // Months are 0-based
}

// Get current file path
const currentFilePath = dv.current().file.path;

// Get all pages with the tag
const pages = dv.pages('#ProjectManagement/Task')
    // Filter for tasks with dealines within the current sprint start and end points
    .filter(page => page["Due-Date"] && parseMMDDYY(formatDate(dv.date(page["Due-Date"]))) > parseMMDDYY(formatDate(dv.date(dv.current()["Sprint-Start"]))) && parseMMDDYY(formatDate(dv.date(page["Due-Date"]))) <= parseMMDDYY(formatDate(dv.date(dv.current()["Sprint-End"]))))

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
    type: 'pie',
    data: {
        labels: labels,
        datasets: [{
            label: 'Number of Tasks',
            data: labels.map(label => statusCounts[label]),
            backgroundColor: labels.map(label => colors[label]),
            borderColor: labels.map(label => borderColors[label]),
            borderWidth: 1,
            hoverOffset: 4
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
### Evaluation Accuracy
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

// Get current file path
const currentFilePath = dv.current().file.path;

// Get all work log entries linked to this person
const workLogs = dv.pages("#ProjectManagement/WorkLogEntry")
	.filter(page => 
        page["Time-Start"] && 
        page["Time-End"] && 
        dv.date(page["Time-Start"]) > dv.date(dv.current()["Sprint-Start"]) && 
        dv.date(page["Time-End"]) <= dv.date(dv.current()["Sprint-End"]));

// Calculate data
const roleData = {}; // role name : { roleHoursEstimated, roleTimeInvested, roleColor }
workLogs.forEach(log => {
    if (log["Author"] && log["task"] && log["Time-Start"] && log["Time-End"]) {
        const role = dv.page(log["Author"])["role"] || 'Unassigned';
        const hoursEstimated = dv.page(log["task"])["Hours-Estimated"] || 0;
        const hoursInvested = calculateHours(log["Time-Start"], log["Time-End"]);

        // Initialize if not exists
        if (!roleData[role]) {
            roleData[role] = {
                roleHoursEstimated: 0,
                roleTimeInvested: 0,
                roleColor: 'rgba(255,0,255,1)'
            };
        }

        // Update values
        roleData[role].roleHoursEstimated += hoursEstimated;
        roleData[role].roleTimeInvested += hoursInvested;
    }
});

// Prepare data for the chart
const chartData = {
    type: 'bar',
    data: {
        labels: Object.keys(roleData),
        datasets: [
            {
                label: 'Hours Estimated',
                data: Object.keys(roleData).map(role => Object.values(roleData[role])[0]),
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgb(54, 162, 235)',
                borderWidth: 1,
                borderJoinStyle: 'round',
                borderRadius: 8,
                hoverOffset: 4
            },
            {
                label: 'Hours Invested',
                data: Object.keys(roleData).map(role => Object.values(roleData[role])[1]),
                backgroundColor: 'rgba(235, 162, 54, 0.2)',
                borderColor: 'rgb(235, 162, 54)',
                borderWidth: 1,
                borderJoinStyle: 'round',
                borderRadius: 8,
                hoverOffset: 4
            },
        ]
    },
    options: {
        elements: {
	        line: {
	            borderWidth: 3
	        }
	    }
    }
};

window.renderChart(chartData, this.container);
```
