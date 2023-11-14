import { t } from "i18next";
export const CREATE_GRAPH = {
  XML_FILE : '<mxStylesheet><add as="defaultVertex"><add as="shape" value="label"/><add as="perimeter" value="rectanglePerimeter"/><add as="fontSize" value="12"/><add as="fontFamily" value="Helvetica"/><add as="align" value="center"/><add as="verticalAlign" value="middle"/><add as="fillColor" value="#ffffff"/><add as="strokeColor" value="#000000"/><add as="fontColor" value="#000000"/></add><add as="defaultEdge"><add as="shape" value="connector"/><add as="labelBackgroundColor" value="#ffffff"/><add as="endArrow" value="classic"/><add as="fontSize" value="11"/><add as="fontFamily" value="Helvetica"/><add as="align" value="center"/><add as="verticalAlign" value="middle"/><add as="rounded" value="1"/><add as="strokeColor" value="#000000"/><add as="fontColor" value="#000000"/></add><add as="text"><add as="fillColor" value="none"/><add as="gradientColor" value="none"/><add as="strokeColor" value="none"/><add as="align" value="left"/><add as="verticalAlign" value="top"/></add><add as="edgeLabel" extend="text"><add as="labelBackgroundColor" value="#ffffff"/><add as="fontSize" value="11"/></add><add as="label"><add as="fontStyle" value="1"/><add as="align" value="left"/><add as="verticalAlign" value="middle"/><add as="spacing" value="2"/><add as="spacingLeft" value="52"/><add as="imageWidth" value="42"/><add as="imageHeight" value="42"/><add as="rounded" value="1"/></add><add as="icon" extend="label"><add as="align" value="center"/><add as="imageAlign" value="center"/><add as="verticalLabelPosition" value="bottom"/><add as="verticalAlign" value="top"/><add as="spacingTop" value="4"/><add as="labelBackgroundColor" value="#ffffff"/><add as="spacing" value="0"/><add as="spacingLeft" value="0"/><add as="spacingTop" value="6"/><add as="fontStyle" value="0"/><add as="imageWidth" value="48"/><add as="imageHeight" value="48"/></add><add as="swimlane"><add as="shape" value="swimlane"/><add as="fontSize" value="12"/><add as="fontStyle" value="1"/><add as="startSize" value="23"/></add><add as="group"><add as="verticalAlign" value="top"/><add as="fillColor" value="none"/><add as="strokeColor" value="none"/><add as="gradientColor" value="none"/><add as="pointerEvents" value="0"/></add><add as="ellipse"><add as="shape" value="ellipse"/><add as="perimeter" value="ellipsePerimeter"/></add><add as="rhombus"><add as="shape" value="rhombus"/><add as="perimeter" value="rhombusPerimeter"/></add><add as="triangle"><add as="shape" value="triangle"/><add as="perimeter" value="trianglePerimeter"/></add><add as="line"><add as="shape" value="line"/><add as="strokeWidth" value="4"/><add as="labelBackgroundColor" value="#ffffff"/><add as="verticalAlign" value="top"/><add as="spacingTop" value="8"/></add><add as="image"><add as="shape" value="image"/><add as="labelBackgroundColor" value="white"/><add as="verticalAlign" value="top"/><add as="verticalLabelPosition" value="bottom"/></add><add as="roundImage" extend="image"><add as="perimeter" value="ellipsePerimeter"/></add><add as="rhombusImage" extend="image"><add as="perimeter" value="rhombusPerimeter"/></add><add as="arrow"><add as="shape" value="arrow"/><add as="edgeStyle" value="none"/><add as="fillColor" value="#ffffff"/></add></mxStylesheet>',
}

export const ICON_TYPE = {
  START: 0,
  END: 1,
  JOB: 4,
  JOBNET: 5,
  CONDITIONAL_START: 2,
  CONDITIONAL_END: 13,
  PARALLEL_START: 6,
  PARALLEL_END: 7,
  JOB_CONTROL_VARIABLE: 3,
  EXTENDED_JOB: 9,
  CALCULATION: 10,
  LOOP: 8,
  TASK: 11,
  INFO: 12,
  FILE_COPY: 14,
  FILE_WAIT: 15,
  REBOOT: 16,
  RELEASE: 17,
  AGENT_LESS: 18,
  ZABBIX: 19,
  EXECUTE_JOB: 20, // it's a fake icon type for execute job.
  VIEW_VAR_VALUE: 21, // it's a fake icon type for view variable value,
  VAR_VALUE_CHANGE: 22, // it's a fake icon type for variable value change
};

export const MXGRAPH = {
  STYLE_FILLCOLOR: "fillColor",
  STYLE_FONTCOLOR: "fontColor",
};

export const GRAPH = {
  JOBNET_MANAGE_GRAPH: "Jobnet Manage Graph",
  JOBNET_EXEC_GRAPH: "Jobnet Exec Graph",
  JOBNET_ICON_GRAPH: "Jobnet Icon Graph",
};

export const VERTEX_COLORS = {
  AQUAMARINE: "#7fffd4", // light blue
  PURPLE: "#b653cf", // purple
  GRAY: "#808080", // gray
  ORANGE: "#FFA500", // orange
  YELLOW: "#FFFF00", // yellow
  GREEN: "#00FF00", // green
  RED: "#FF0000", // red
  WHITE: "#ffffff",
  BLACK: "#000000",
};

export const INVALID_STRING = {
  START: "START",
};

export const JOB_ID_LIST = {
  4: {
    id: "JOB",
    count: 0,
  },
  2: {
    id: "IF",
    count: 0,
  },
  13: {
    id: "IFE",
    count: 0,
  },
  6: {
    id: "MTS",
    count: 0,
  },
  7: {
    id: "MTE",
    count: 0,
  },
  3: {
    id: "ENV",
    count: 0,
  },
  9: {
    id: "EXTJOB",
    count: 0,
  },
  0: {
    id: "START",
    count: null,
  },
  1: {
    id: "END",
    count: 0,
  },
  10: {
    id: "CAL",
    count: 0,
  },
  8: {
    id: "LOOP",
    count: 0,
  },
  11: {
    id: "TASK",
    count: 0,
  },
  12: {
    id: "INFO",
    count: 0,
  },
  14: {
    id: "FCOPY",
    count: 0,
  },
  15: {
    id: "FWAIT",
    count: 0,
  },
  16: {
    id: "REBOOT",
    count: 0,
  },
  17: {
    id: "REL",
    count: 0,
  },
  18: {
    id: "LESS",
    count: 0,
  },
  19: {
    id: "ZABBIX",
    count: 0,
  },
};
