// Fork viewer implementation from enhancedforkgraph.txt
import * as d3 from 'd3';
import * as dagre from 'dagre';

let svg;
let g;
let zoom;
let simulation;
let currentLayout = 'force';
let currentGrouping = 'none';
let legend;

const nodeWidth = 240;
const nodeHeight = 100;
const margin = { top: 20, right: 90, bottom: 30, left: 90 };

// Color scales for different groupings
const authorColors = d3.scaleOrdinal(d3.schemeCategory10);
const themeColors = d3.scaleOrdinal(d3.schemePaired);
const timeColors = d3.scaleSequential(d3.interpolateViridis);

export function initForkGraph(containerId, data, layoutType, groupingMode, showLegend, dotNetHelper) {
    currentLayout = layoutType;
    currentGrouping = groupingMode;

    const container = d3.select(`#${containerId}`);
    const width = container.node().getBoundingClientRect().width;
    const height = container.node().getBoundingClientRect().height;

    // Clear existing content
    container.selectAll("*").remove();

    // Create new SVG
    svg = container.append("svg")
        .attr("width", width)
        .attr("height", height);

    // Add zoom behavior
    zoom = d3.zoom()
        .scaleExtent([0.5, 2])
        .on("zoom", (event) => {
            g.attr("transform", event.transform);
        });

    svg.call(zoom);

    // Create the root group
    g = svg.append("g")
        .attr("transform", `translate(${margin.left},${margin.top})`);

    // Add arrow markers
    addArrowMarkers();

    // Process data based on grouping
    const processedData = processDataForGrouping(data, groupingMode);

    // Create the visualization based on layout type
    if (layoutType === 'force') {
        createForceLayout(processedData, dotNetHelper);
    } else {
        createDagreLayout(processedData, dotNetHelper);
    }

    if (showLegend) {
        createLegend(processedData, groupingMode);
    }
}

// Rest of the fork graph implementation from enhancedforkgraph.txt