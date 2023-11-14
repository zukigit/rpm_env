import { openIconSettingDialog } from "../../store/JobnetFormSlice";
import _ from "lodash";
import $ from "jquery";
import { v4 as uuidv4 } from "uuid";
import {
  ICON_TYPE,
  MXGRAPH,
  RUN_JOB_METHOD_TYPE,
  RUN_JOB_STATUS_TYPE,
  RUN_JOB_TIMEOUT_TYPE,
  VERTEX_COLORS,
} from "../../constants/index";
import { arranageIconSetting } from "./ArrangeIconData";
import i18next, { t } from "i18next";
import { openExecIconSettingDialog } from "../../store/JobExecutionSlice";

export const VertexOnDblclick = (
  evt,
  state,
  graph,
  t,
  dispatch,
  graphIndexId
) => {
  var cell;
  if (!state.hasOwnProperty("cellType")) {
    cell = state.cell;
  } else {
    cell = state;
  }
  if (cell.dValue === "Child Icon") {
    return
  }
  if (graph.isEnabled()) {
    var isJobnetSetting = false;
    if (cell.cellType === ICON_TYPE.JOBNET && evt === undefined) {
      isJobnetSetting = true;
    }
    dispatch(
      openIconSettingDialog(
        graphIndexId,
        {
          key: uuidv4(),
          id: cell.id,
          cellType: cell.cellType,
          jobId: cell.jobId,
          jobName: cell.jobName,
          iconSetting: cell.iconSetting,
          isJobnetSetting
        }
      )
    );
  }
  graph.tooltipHandler.hideTooltip();
};

export const getTranslateObject = () => {
  const t = i18next.t;
  return t;
}

export const getResourceTxt = () => {
  return `alreadyConnected=Nodes already connected\ncancel=Cancel\nclose=Close\ncollapse-expand=Collapse/Expand\ncontainsValidationErrors=Contains validation errors\ndone=Done\ndoubleClickOrientation=Doubleclick to Change Orientation\nerror=Error\nerrorSavingFile=Error saving file\nok=OK\nupdatingDocument=Updating Document. Please wait...\nupdatingSelection=Updating Selection. Please wait...\n# Custom resources\nabout=About\nactualSize=Actual Size\nadd=Add\naddLayer=Add Layer\naddProperty=Add Property\naddToExistingDrawing=Add to Existing Drawing\naddWaypoint=Add Waypoint\nadvanced=Advanced\nalign=Align\nalignment=Alignment\nallChangesLost=All changes will be lost!\nangle=Angle\napply=Apply\narc=Arc\narrange=Arrange\narrow=Arrow\narrows=Arrows\nautomatic=Automatic\nautosave=Autosave\nautosize=Autosize\nback=Back\nbackground=Background\nbackgroundColor=Background Color\nbackgroundImage=Background Image\nbasic=Basic\nblock=Block\nblockquote=Blockquote\nbold=Bold\nborder=Border\nborderWidth=Borderwidth\nborderColor=Border Color\nbottom=Bottom\nbottomAlign=Bottom Align\nbottomLeft=Bottom Left\nbottomRight=Bottom Right\nbulletedList=Bulleted List\ncannotOpenFile=Cannot open file\ncenter=Center\nchange=Change\nchangeOrientation=Change Orientation\ncircle=Circle\nclassic=Classic\nclearDefaultStyle=Clear Default Style\nclearWaypoints=Clear Waypoints\nclipart=Clipart\ncollapse=Collapse\ncollapseExpand=Collapse/Expand\ncollapsible=Collapsible\ncomic=Comic\nconnect=Connect\nconnection=Connection\nconnectionPoints=Connection points\nconnectionArrows=Connection arrows\nconstrainProportions=Constrain Proportions\ncopy=Copy\ncopyConnect=Copy on Connect\ncopySize=Copy Size\ncreate=Create\ncurved=Curved\ncustom=Custom\ncut=Cut\ndashed=Dashed\ndecreaseIndent=Decrease Indent\ndefault=Default\nselectAll=SelectAll\ndelete=${t("col-delete")}\ndeleteColumn=Delete Column\ndeleteRow=Delete Row\ndiagram=Diagram\ndiamond=Diamond\ndiamondThin=Diamond (thin)\ndirection=Direction\ndistribute=Distribute\ndivider=Divider\ndocumentProperties=Document Properties\ndotted=Dotted\ndpi=DPI\ndrawing=Drawing{1}\ndrawingEmpty=Drawing is empty\ndrawingTooLarge=Drawing is too large\nduplicate=Duplicate\nduplicateIt=Duplicate {1}\neast=East\nedit=Edit\neditData=Edit Data\neditDiagram=Edit Diagram\neditImage=Edit Image\neditLink=Edit Link\neditStyle=Edit Style\neditTooltip=Edit Tooltip\nenterGroup=Enter Group\nenterValue=Enter Value\nenterName=Enter Name\nenterPropertyName=Enter Property Name\nentityRelation=Entity Relation\nexitGroup=Exit Group\nexpand=Expand\nexport=Export\nextras=Extras\nfile=File\nfileNotFound=File not found\nfilename=Filename\nfill=Fill\nfillColor=Fill Color\nfitPage=One Page\nfitPageWidth=Page Width\nfitTwoPages=Two Pages\nfitWindow=Fit Window\nflip=Flip\nflipH=Flip Horizontal\nflipV=Flip Vertical\nfont=Font\nfontFamily=Font Family\nfontColor=Font Color\nfontSize=Font Size\nformat=Format\nformatPanel=Format Panel\ngeneral=Allgemein\nforceStop=${t("btn-force-stop")}\nformatPdf=PDF\nformatPng=PNG\nformatGif=GIF\nformatJpg=JPEG\nformatSvg=SVG\nformatXml=XML\nformatted=Formatted\nformattedText=Formatted Text\ngap=Gap\nglass=Glass\ngeneral=General\nglobal=Global\ngradient=Gradient\ngradientColor=Color\ngrid=Grid\ngridSize=Grid Size\ngroup=Group\nguides=Guides\nheading=Heading\nheight=Height\nhelp=Help\nhide=Hide\nhideIt=Hide {1}\nhidden=Hidden\nhold=${t("btn-hold")}\nhome=Home\nhorizontal=Horizontal\nhorizontalFlow=Horizontal Flow\nhorizontalTree=Horizontal Tree\nhtml=HTML\nid=ID\nimage=Image\nimages=Images\nimport=Import\nincreaseIndent=Increase Indent\ninsert=Insert\ninsertColumnBefore=Insert Column Left\ninsertColumnAfter=Insert Column Right\ninsertHorizontalRule=Insert Horizontal Rule\ninsertImage=Insert Image\ninsertLink=Insert Link\ninsertRowBefore=Insert Row Above\ninsertRowAfter=Insert Row Below\ninvalidInput=Invalid input\ninvalidName=Invalid name\ninvalidOrMissingFile=Invalid or missing file\nisometric=Isometric\nitalic=Italic\njobRun=${t("btn-job-run")}\nlayers=Layers\nlandscape=Landscape\nlaneColor=Lanecolor\nlayout=Layout\nleft=Left\nleftAlign=Left Align\nleftToRight=Left to Right\nline=Line\nlink=Link\nlineJumps=Line jumps\nlineend=Line End\nlineheight=Line Height\nlinestart=Line Start\nlinewidth=Linewidth\nloading=Loading\nlockUnlock=Lock/Unlock\nmanual=Manual\nmethodFlagHold=${t("btn-hold")}\nmethodFlagUnhold=${t("btn-unhold")}\nmethodFlagSkip=${t("sel-skip")}\nmethodFlagUnskip=${t("sel-unskip")}\nmethodFlagTrue=${"True"}\nmethodFlagFalse=${"False"}\nmiddle=Middle\nmisc=Misc\nmore=More\nmoreResults=More Results\nmove=Move\nmoveSelectionTo=Move Selection to {1}\nnavigation=Navigation\nnew=New\nnoColor=No Color\nnoFiles=No files\nnoMoreResults=No more results\nnone=None\nnoResultsFor=No results for '{1}'\nnormal=Normal\nnorth=North\nnumberedList=Numbered List\nopacity=Opacity\nopen=Open\nopenArrow=Open Arrow\nopenFile=Open File\nopenLink=Open Link\nopenSupported=Supported format is .XML files saved from this software\nopenInNewWindow=Open in New Window\nopenInThisWindow=Open in this Window\noptions=Options\norganic=Organic\northogonal=Orthogonal\noutline=Outline\noval=Oval\npages=Pages\npageView=Page View\npageScale=Page Scale\npageSetup=Page Setup\npanTooltip=Space+Drag to Scroll\npaperSize=Paper Size\npaste=Paste\npasteHere=Paste Here\npasteSize=Paste Size\npattern=Pattern\nperimeter=Perimeter\nplaceholders=Placeholders\nplusTooltip=Click to connect and clone (ctrl+click to clone, shift+click to connect). Drag to connect (ctrl+drag to clone).\nportrait=Portrait\nposition=Position\nposterPrint=Poster Print\npreview=Preview\nprint=Print\nradialTree=Radial Tree\nredo=Redo\nremoveFormat=Clear Formatting\nremoveFromGroup=Remove from Group\nremoveIt=Remove {1}\nremoveWaypoint=Remove Waypoint\nrename=Rename\nrenameIt=Rename {1}\nreplace=Replace\nreplaceIt={1} already exists. Do you want to replace it?\nreplaceExistingDrawing=Replace existing drawing\nrerun=${t("btn-rerun")}\nreset=Reset\nresetView=Reset View\nreverse=Reverse\nright=Right\nrightAlign=Right Align\nrightToLeft=Right to Left\nrotate=Rotate\nrotateTooltip=Click and drag to rotate, click to turn shape only by 90 degrees\nrotation=Rotation\nsketch=Sketch\nrounded=Rounded\nsave=Save\nsaveAs=Save as\nsaved=Saved\nscrollbars=Scrollbars\nsearch=Search\nsearchShapes=Search Shapes\nselectAll=Select All\nselectEdges=Select Edges\nselectFont=Select a Font\nselectNone=Select None\nselectVertices=Select Vertices\nsetAsDefaultStyle=Set as Default Style\nsetting=${t("btn-setting")}\nshadow=Shadow\nshape=Shape\nsharp=Sharp\nsidebarTooltip=Click to expand. Drag and drop shapes into the diagram. Shift+click to change selection. Alt+click to insert and connect.\nsimple=Simple\nsimpleArrow=Simple Arrow\nsize=Size\nskip=${t("sel-skip")}\nsolid=Solid\nsourceSpacing=Source Spacing\nsouth=South\nspacing=Spacing\nstraight=Straight\nstrikethrough=Strikethrough\nstrokeColor=Line Color\nstyle=Style\nsubscript=Subscript\nsuperscript=Superscript\ntable=Table\ntargetSpacing=Target Spacing\ntext=Text\ntextAlignment=Text Alignment\ntextOpacity=Text Opacity\ntoBack=To Back\ntoFront=To Front\ntooltips=Tooltips\ntop=Top\ntopAlign=Top Align\ntopLeft=Top Left\ntopRight=Top Right\ntransparent=Transparent\nturn=Rotate shape only by 90°\numl=UML\nunderline=Underline\nundo=Undo\nungroup=Ungroup\nunhold=${t("btn-unhold")}\nunskip=${t("sel-unskip")}\nurl=URL\nuntitledLayer=Untitled Layer\nvariableValueChange=${t("menu-view-var-value")}\nvertical=Vertical\nverticalFlow=Vertical Flow\nverticalTree=Vertical Tree\nview=View\nviewVariableValue=${t("title-view-var-value")}\nwaypoints=Waypoints\nwest=West\nwidth=Width\nwordWrap=Word Wrap\nwritingDirection=Writing Direction\nzoom=Zoom\nzoomIn=Zoom In\nzoomOut=Zoom Out\n`;
}

export const ExecVertexOnDblclick = (
  evt,
  state,
  graph,
  t,
  dispatch,
  innerJobnetMainId
) => {
  var cell;
  if (!state.hasOwnProperty("cellType")) {
    cell = state.cell;
  } else {
    cell = state;
  }
  if (graph.isEnabled()) {
    dispatch(
      openExecIconSettingDialog(
        {
          id: cell.id,
          cellType: cell.cellType,
          jobId: cell.jobId,
          jobName: cell.jobName,
          iconSetting: cell.iconSetting,
          innerJobnetId: cell.iconSetting.linkInnerJobnetId
        },
        innerJobnetMainId
      )
    );
  }
  graph.tooltipHandler.hideTooltip();
};

export const ClearGraph = (graph) => {
  graph.model.beginUpdate();
  try {
    var parent = graph.getDefaultParent();
    graph.cellsRemoved(parent.children);
    graph.removeCells(graph.getChildVertices(graph.getDefaultParent()));
    graph.getModel().clear();
  } finally {
    graph.model.endUpdate();
  }
};

export const AddDataGraph = (dataArray, graph, editor) => {
  const t = i18next.t;
  var parent = graph.getDefaultParent();
  var jobsCount = dataArray.jobs.length;
  let vertexObj = {};

  dataArray.jobs.map((value) => {
    // var childCell = graph.childCellGen(value.jobData.job_type);
    let iconSetting = _.mapKeys(value.iconSetting, (value, key) =>
      _.camelCase(key)
    );

    let job = {
      ...value,
      iconSetting: iconSetting,
    };
    var jobDisplayText = `<div style="height: 15px; width: 80px; overflow:hidden; white-space: nowrap; text-overflow: ellipsis;">${job.jobData.job_id}</div>`;
    if (job.jobData.job_name) {
      jobDisplayText += `<div style="height:15px; width: 80px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;">${job.jobData.job_name}</div>`;
    }
    if (
      graph.jobTypeList[job.jobData.job_type].cellType ===
      ICON_TYPE.CONDITIONAL_END
    ) {
      jobDisplayText = "|";
    } else if (
      graph.jobTypeList[job.jobData.job_type].cellType === ICON_TYPE.LOOP
    ) {
      jobDisplayText = "L";
    } else if (
      graph.jobTypeList[job.jobData.job_type].cellType ===
      ICON_TYPE.PARALLEL_START ||
      graph.jobTypeList[job.jobData.job_type].cellType ===
      ICON_TYPE.PARALLEL_END
    ) {
      jobDisplayText = "E";
    }
    let arrangeData = arranageIconSetting(job, graph);

    if (graph.jobTypeList[job.jobData.job_type].cellType === ICON_TYPE.START) {
      graph.isStartIconExist = true;
    }
    let jobData = {
      cellType: graph.jobTypeList[job.jobData.job_type].cellType,
      dValue: graph.jobTypeList[job.jobData.job_type].dValue,
      iconSetting: arrangeData[0],
      toolTipLabel: arrangeData[1],
      beforeVariables: null,
      afterVariables: null,
      jobId: job.jobData.job_id,
      jobName: job.jobData.job_name,
      runStatus: null,
      innerJobnetMainId: null,
      innerJobnetId: null,
      innerJobId: null,
      methodFlag: job.jobData.method_flag,
      jobnetAbortFlag: null,
      jobnetRunStatus: null
    }
    let vertex = graph.insertVertex(
      parent,
      null,
      jobDisplayText,
      parseInt(job.jobData.point_x),
      parseInt(job.jobData.point_y),
      graph.jobTypeList[job.jobData.job_type].width,
      graph.jobTypeList[job.jobData.job_type].height,
      graph.jobTypeList[job.jobData.job_type].style,
      undefined,
      jobData
    );

    if (parseInt(job.jobData.method_flag) !== 0) {
      var cellStyle = graph.model.getStyle(vertex);
      if (cellStyle === null) {
        cellStyle = "";
      }
      if (parseInt(job.jobData.method_flag) === 1) {
        cellStyle = window.mxUtils.setStyle(
          cellStyle,
          MXGRAPH.STYLE_FILLCOLOR,
          "#ba55d3"
        );

        cellStyle = window.mxUtils.setStyle(
          cellStyle,
          MXGRAPH.STYLE_FONTCOLOR,
          "#ffffff"
        );
      }

      if (parseInt(job.jobData.method_flag) === 2) {
        cellStyle = window.mxUtils.setStyle(
          cellStyle,
          MXGRAPH.STYLE_FILLCOLOR,
          "#808080"
        );

        cellStyle = window.mxUtils.setStyle(
          cellStyle,
          MXGRAPH.STYLE_FONTCOLOR,
          "#ffffff"
        );
      }
      graph.model.setStyle(vertex, cellStyle);
    }

    vertexObj[job.jobData.job_id] = vertex;
    if (!--jobsCount) {
      var flowsCount = dataArray.flows.length;
      $.each(dataArray.flows, function (key, value) {
        let flow = value;
        var flowDisplayText = null;
        if (parseInt(flow.flow_type) === 1) {
          flowDisplayText = "True";
        } else if (parseInt(flow.flow_type) === 2) {
          flowDisplayText = "False";
        }

        if (flow.flow_style !== null && flow.flow_style !== "(null)") {
          let flowStyleData = $.parseJSON(flow.flow_style);
          graph.insertEdge(
            parent,
            null,
            flowDisplayText,
            vertexObj[flow.start_job_id],
            vertexObj[flow.end_job_id],
            flowStyleData.style,
            flow.flow_type,
            flowStyleData.points
          );
        } else {
          if (parseInt(flow.flow_width) === 0) {
            graph.insertEdge(
              parent,
              null,
              flowDisplayText,
              vertexObj[flow.start_job_id],
              vertexObj[flow.end_job_id],
              "edgeStyle=none;rounded=0;orthogonalLoop=1;bendable=0;jettySize=auto;html=1;",
              flow.flow_type
            );
          } else {
            let gapX =
              vertexObj[flow.start_job_id].geometry.x -
              vertexObj[flow.end_job_id].geometry.x;
            let gapY =
              vertexObj[flow.start_job_id].geometry.y -
              vertexObj[flow.end_job_id].geometry.y;

            let angle = gapY / gapX;

            // 右⇒左
            if (gapX > 0 && angle > -1 && angle < 1) {
              graph.insertEdge(
                parent,
                null,
                flowDisplayText,
                vertexObj[flow.start_job_id],
                vertexObj[value.end_job_id],
                "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;entryX=0.5;entryY=1;entryDx=0;entryDy=0;exitX=0.5;exitY=1;exitDx=0;exitDy=0;db=1;",
                flow.flow_type
              );
            }
            // 左⇒右
            else if (gapX < 0 && angle > -1 && angle < 1) {
              graph.insertEdge(
                parent,
                null,
                flowDisplayText,
                vertexObj[flow.start_job_id],
                vertexObj[value.end_job_id],
                "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;exitX=0.5;exitY=0;exitDx=0;exitDy=0;entryX=0.5;entryY=0;entryDx=0;entryDy=0;db=1;",
                flow.flow_type
              );
            }
            // 下⇒上
            else if (gapY > 0 && (angle > 1 || angle < -1)) {
              graph.insertEdge(
                parent,
                null,
                flowDisplayText,
                vertexObj[flow.start_job_id],
                vertexObj[flow.end_job_id],
                "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;entryX=0;entryY=0.5;entryDx=0;entryDy=0;exitX=0;exitY=0.5;exitDx=0;exitDy=0;db=1;",
                flow.flow_type
              );
            }
            // 上⇒下
            else if (gapY < 0 && (angle > 1 || angle < -1)) {
              graph.insertEdge(
                parent,
                null,
                flowDisplayText,
                vertexObj[flow.start_job_id],
                vertexObj[flow.end_job_id],
                "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;entryX=1;entryY=0.5;entryDx=0;entryDy=0;exitX=1;exitY=0.5;exitDx=0;exitDy=0;db=1;",
                flow.flow_type
              );
            }
          }
        }

        if (!--flowsCount) {
          editor.undoManager.clear();
        }
      });
    }
  });
};

export function updateVertex(editor, jobnetInfo) {
  let graph = editor.graph;
  $.each(jobnetInfo.runJob, function (key, jobItem) {
    let [beforeVariables, afterVariables] = removeAllInvalidCharInVariables(jobItem.beforeVariable, jobItem.afterVariable);
    let cellId = (parseInt(jobItem.jobData.inner_job_id) == 1) ? "i-1" : jobItem.jobData.inner_job_id;
    let cell = graph.getModel().getCell(cellId);
    jobItem.iconSetting = _.mapKeys(jobItem.iconSetting, (value, key) =>
      _.camelCase(key)
    );

    let toolTipLabel = jobItem.isErrorTooltip ? jobItem.toolTip : arranageIconSetting(jobItem, graph)[1];
    graph.model.setStyle(
      cell,
      graph.jobTypeList[jobItem.jobData.job_type].style +
      "fillColor=" +
      getFillColor(jobItem.jobData) +
      ";fontColor=" +
      getFontColor(jobItem.jobData)
    );

    graph.model.setLatestVertexData(cell, {
      toolTipLabel,
      beforeVariables,
      afterVariables,
      runStatus: jobItem.jobData.status,
      methodFlag: jobItem.jobData.method_flag,
      jobnetAbortFlag: jobItem.jobData.jobnet_abort_flag,
      jobnetRunStatus: jobnetInfo.runJobSummary
        ? jobnetInfo.runJobSummary.status
        : jobnetInfo.runJobnet.status
    })
  });
}

export function displayVertexAndFlow(editor, jobnetInfo) {
  const t = i18next.t;
  let graph = editor.graph;
  let jobsCount = jobnetInfo.runJob.length;
  let vertexObj = {};
  $.each(jobnetInfo.runJob, function (key, jobItem) {
    jobItem.iconSetting = _.mapKeys(jobItem.iconSetting, (value, key) =>
      _.camelCase(key)
    );
    var jobDisplayText = `<div style="height: 15px; width: 80px; overflow:hidden; white-space: nowrap; text-overflow: ellipsis;">${jobItem.jobData.job_id}</div>`;
    if (jobItem.jobData.job_name) {
      jobDisplayText += `<div style="height: 15px; width: 80px; overflow:hidden; white-space: nowrap; text-overflow: ellipsis;">${jobItem.jobData.job_name}</div>`;
    }
    if (
      graph.jobTypeList[jobItem.jobData.job_type].cellType ===
      ICON_TYPE.CONDITIONAL_END
    ) {
      jobDisplayText = "|";
    } else if (
      graph.jobTypeList[jobItem.jobData.job_type].cellType === ICON_TYPE.LOOP
    ) {
      jobDisplayText = "L";
    } else if (
      graph.jobTypeList[jobItem.jobData.job_type].cellType ===
      ICON_TYPE.PARALLEL_START ||
      graph.jobTypeList[jobItem.jobData.job_type].cellType ===
      ICON_TYPE.PARALLEL_END
    ) {
      jobDisplayText = "E";
    }
    let arrangeData = arranageIconSetting(jobItem, graph);

    let [beforeVariables, afterVariables] = removeAllInvalidCharInVariables(jobItem.beforeVariable, jobItem.afterVariable);
    let jobData = {
      cellType: graph.jobTypeList[jobItem.jobData.job_type].cellType,
      dValue: graph.jobTypeList[jobItem.jobData.job_type].dValue,
      iconSetting: arrangeData[0],
      toolTipLabel: jobItem.isErrorTooltip ? jobItem.toolTip : arrangeData[1],
      beforeVariables,
      afterVariables,
      jobId: jobItem.jobData.job_id,
      jobName: jobItem.jobData.job_name,
      runStatus: jobItem.jobData.status,
      innerJobnetMainId: jobItem.jobData.inner_jobnet_main_id,
      innerJobnetId: jobItem.jobData.inner_jobnet_id,
      innerJobId: jobItem.jobData.inner_job_id,
      methodFlag: jobItem.jobData.method_flag,
      jobnetAbortFlag: jobItem.jobData.jobnet_abort_flag,
      jobnetRunStatus: jobnetInfo.runJobSummary
        ? jobnetInfo.runJobSummary.status
        : jobnetInfo.runJobnet.status
    }
    let cellId = (parseInt(jobItem.jobData.inner_job_id) == 1) ? "i-1" : jobItem.jobData.inner_job_id;
    let vertex = graph.insertVertex(
      graph.getDefaultParent(),
      cellId,
      jobDisplayText,
      parseInt(jobItem.jobData.point_x),
      parseInt(jobItem.jobData.point_y),
      graph.jobTypeList[jobItem.jobData.job_type].width,
      graph.jobTypeList[jobItem.jobData.job_type].height,
      graph.jobTypeList[jobItem.jobData.job_type].style +
      "fillColor=" +
      getFillColor(jobItem.jobData) +
      ";fontColor=" +
      getFontColor(jobItem.jobData),
      undefined,
      jobData
    );

    vertexObj[jobItem.jobData.inner_job_id] = vertex;

    if (!--jobsCount) {
      displayFlow(editor, jobnetInfo, vertexObj);
    }
  });
}

function removeAllInvalidCharInVariables(beforeVariables, afterVariables) {
  let newBeforeVariables = beforeVariables.map(beforeVar => {
    beforeVar.before_value = beforeVar.before_value.replaceAll("", "")
    return beforeVar
  })
  let newAfterVariables = afterVariables.map(afterVar => {
    afterVar.after_value = afterVar.after_value.replaceAll("", "")
    return afterVar
  })
  return [newBeforeVariables, newAfterVariables]
}

export function displayFlow(editor, jobnetInfo, vertexObj) {
  var flowsCount = jobnetInfo.runFlow.length;
  $.each(jobnetInfo.runFlow, function (key, flow) {
    var flowDisplayText = null;
    if (parseInt(flow.flow_type) === 1) {
      flowDisplayText = "True";
    } else if (parseInt(flow.flow_type) === 2) {
      flowDisplayText = "False";
    }
    if (flow.flow_style != null && flow.flow_style != "(null)") {
      let flowStyleData = $.parseJSON(flow.flow_style);
      editor.graph.insertEdge(
        editor.graph.getDefaultParent(),
        null,
        flowDisplayText,
        vertexObj[flow.start_inner_job_id],
        vertexObj[flow.end_inner_job_id],
        flowStyleData.style,
        flow.flow_type,
        flowStyleData.points
      );
    } else {
      if (parseInt(flow.flow_width) === 0) {
        editor.graph.insertEdge(
          editor.graph.getDefaultParent(),
          null,
          flowDisplayText,
          vertexObj[flow.start_inner_job_id],
          vertexObj[flow.end_inner_job_id],
          "edgeStyle=none;rounded=0;orthogonalLoop=1;bendable=0;jettySize=auto;html=1;",
          flow.flow_type
        );
      } else {
        displayCurveFlow(editor.graph, vertexObj, flow, flowDisplayText);
      }
    }
    if (!--flowsCount) {
      editor.undoManager.clear();
    }
  });
}

export function displayCurveFlow(graph, vertexObj, flow, flowDisplayText) {
  let gapX =
    vertexObj[flow.start_inner_job_id].geometry.x -
    vertexObj[flow.end_inner_job_id].geometry.x;
  let gapY =
    vertexObj[flow.start_inner_job_id].geometry.y -
    vertexObj[flow.end_inner_job_id].geometry.y;

  let angle = gapY / gapX;

  // 右⇒左
  if (gapX > 0 && angle > -1 && angle < 1) {
    graph.insertEdge(
      graph.getDefaultParent(),
      null,
      flowDisplayText,
      vertexObj[flow.start_inner_job_id],
      vertexObj[flow.end_inner_job_id],
      "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;entryX=0.5;entryY=1;entryDx=0;entryDy=0;exitX=0.5;exitY=1;exitDx=0;exitDy=0;db=1;",
      flow.flow_type
    );
  }
  // 左⇒右
  else if (gapX < 0 && angle > -1 && angle < 1) {
    graph.insertEdge(
      graph.getDefaultParent(),
      null,
      flowDisplayText,
      vertexObj[flow.start_inner_job_id],
      vertexObj[flow.end_inner_job_id],
      "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;exitX=0.5;exitY=0;exitDx=0;exitDy=0;entryX=0.5;entryY=0;entryDx=0;entryDy=0;db=1;",
      flow.flow_type
    );
  }
  // 下⇒上
  else if (gapY > 0 && (angle > 1 || angle < -1)) {
    graph.insertEdge(
      graph.getDefaultParent(),
      null,
      flowDisplayText,
      vertexObj[flow.start_inner_job_id],
      vertexObj[flow.end_inner_job_id],
      "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;entryX=0;entryY=0.5;entryDx=0;entryDy=0;exitX=0;exitY=0.5;exitDx=0;exitDy=0;db=1;",
      flow.flow_type
    );
  }
  // 上⇒下
  else if (gapY < 0 && (angle > 1 || angle < -1)) {
    graph.insertEdge(
      graph.getDefaultParent(),
      null,
      flowDisplayText,
      vertexObj[flow.start_inner_job_id],
      vertexObj[flow.end_inner_job_id],
      "edgeStyle=elbowEdgeStyle;rounded=0;curved=1;jettySize=auto;html=1;entryX=1;entryY=0.5;entryDx=0;entryDy=0;exitX=1;exitY=0.5;exitDx=0;exitDy=0;db=1;",
      flow.flow_type
    );
  }
}

function getFillColor(vertex) {
  let color = VERTEX_COLORS.AQUAMARINE;
  switch (parseInt(vertex.status)) {
    case RUN_JOB_STATUS_TYPE.NONE:
      if (vertex.method_flag == RUN_JOB_METHOD_TYPE.HOLD) {
        color = VERTEX_COLORS.PURPLE;
      }
      if (vertex.method_flag == RUN_JOB_METHOD_TYPE.SKIP) {
        color = VERTEX_COLORS.GRAY;
      }
      break;
    case RUN_JOB_STATUS_TYPE.PREPARE:
      if (vertex.method_flag == RUN_JOB_METHOD_TYPE.HOLD) {
        color = VERTEX_COLORS.PURPLE;
      }
      if (vertex.method_flag == RUN_JOB_METHOD_TYPE.SKIP) {
        color = VERTEX_COLORS.GRAY;
      }
      break;
    case RUN_JOB_STATUS_TYPE.DURING:
      if (vertex.timeout_flag == RUN_JOB_TIMEOUT_TYPE.TIMEOUT) {
        color = VERTEX_COLORS.ORANGE;
      } else {
        color = VERTEX_COLORS.YELLOW;
      }
      break;
    case RUN_JOB_STATUS_TYPE.NORMAL:
      color = VERTEX_COLORS.GREEN;
      if (vertex.method_flag == RUN_JOB_METHOD_TYPE.SKIP) {
        color = VERTEX_COLORS.GRAY;
      } else if (vertex.timeout_flag == RUN_JOB_TIMEOUT_TYPE.TIMEOUT) {
        color = VERTEX_COLORS.ORANGE;
      }
      break;
    case RUN_JOB_STATUS_TYPE.RUN_ERR:
      color = VERTEX_COLORS.RED;
      break;
    case RUN_JOB_STATUS_TYPE.ABNORMAL:
      color = VERTEX_COLORS.RED;
      break;
    case RUN_JOB_STATUS_TYPE.FORCE_STOP:
      if (vertex.timeout_flag == RUN_JOB_TIMEOUT_TYPE.TIMEOUT) {
        color = VERTEX_COLORS.ORANGE;
      } else {
        color = VERTEX_COLORS.YELLOW;
      }
      break;
  }
  return color;
}

function getFontColor(vertex) {
  let color = VERTEX_COLORS.BLACK;
  switch (parseInt(vertex.status)) {
    case RUN_JOB_STATUS_TYPE.NONE:
    case RUN_JOB_STATUS_TYPE.PREPARE:
    case RUN_JOB_STATUS_TYPE.NORMAL:
      if (vertex.method_flag == RUN_JOB_METHOD_TYPE.SKIP) {
        color = VERTEX_COLORS.WHITE;
      }
      break;
  }
  return color;
}

export function addOverlap(container, overlay, overlayClassName) {
  if (!overlay) {
    overlay = document.createElement("div");
    overlay.className = overlayClassName;
    container.appendChild(overlay);
  } else {
    overlay.parentNode.removeChild(overlay);
  }
}
