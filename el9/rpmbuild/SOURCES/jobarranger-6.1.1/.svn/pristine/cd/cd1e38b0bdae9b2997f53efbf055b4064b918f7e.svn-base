import i18next from "i18next";
import { getShiftJISByteLength } from "../../common/Util";
import { alertError } from "../../components/dialogs/CommonDialog";
import { ICON_TYPE, REGEX_PATTERM, SERVICE_RESPONSE } from "../../constants/index";
import JobnetService from "../../services/JobnetService";

/**
 * Function: validateAlertMsg
 *
 * Custom validate error message when user clicks the save button.
 *
 * Parameters:
 *
 * cell - <mxCell> whose icon is occured validation error.
 */
function validateAlertMsg(t, cell) {
  alertError(t("title-error"), "Please setup the icon " + cell.jobId + ".");
}

/**
 * Function: validateJobIconSetting
 *
 * validates the job icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateJobIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.hostName) {
    valid = false;
  }

  if (!cell.iconSetting.exec) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }
  return valid;
}

/**
 * Function: validateFileWaitIconSetting
 *
 * validates the file wait icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateFileWaitIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.hostName) {
    valid = false;
  }

  if (!cell.iconSetting.fileName) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateRebootIconSetting
 *
 * validates the reboot icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateRebootIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.hostName) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateReleaseIconSetting
 *
 * validates the release icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateReleaseIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.releaseUnholdJobId) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateZabbixIconSetting
 *
 * validates the zabbix icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateZabbixIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (cell.iconSetting.groupId === undefined) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateFileTransferIconSetting
 *
 * validates the file transfer icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateFileTransferIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.fromHostName) {
    valid = false;
  }

  if (!cell.iconSetting.toHostName) {
    valid = false;
  }

  if (!cell.iconSetting.fromDirectory) {
    valid = false;
  }

  if (!cell.iconSetting.toDirectory) {
    valid = false;
  }

  if (!cell.iconSetting.fromFileName) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: (cell.icon) && IconSetting
 *
 * validates the agentless icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateAgentLessIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if ((cell.iconSetting.sessionFlag == 0 || cell.iconSetting.sessionFlag == 1) && !cell.iconSetting.hostName) {
    valid = false;
  }

  if ((cell.iconSetting.sessionFlag == 1 || cell.iconSetting.sessionFlag == 2 || cell.iconSetting.sessionFlag == 3) && !cell.iconSetting.sessionId) {
    valid = false;
  }

  if (cell.iconSetting.sessionFlag === null) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateIfIconSetting
 *
 * validates the conditaional start icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateIfIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  // check variable
  if (!cell.iconSetting.variable) {
    valid = false;
  }

  // check comparison variable
  if (!cell.iconSetting.comparisonValue) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateEndIfIconSetting
 *
 * validates the conditional end icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateEndIfIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateParallelEndIconSetting
 *
 * validates the parallel end icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateParallelEndIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateParallelStartIconSetting
 *
 * validates the parallel end icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateParallelStartIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateJobConVarIconSetting
 *
 * validates the job control variable icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateJobConVarIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if(cell.iconSetting.hasOwnProperty("valueJob")){
    if (cell.iconSetting.valueJob.length === 0) {
      valid = false;
    }
  }else{
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateExtendedIconSetting
 *
 * validates the extended icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateExtendedIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.commandId) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateEndIconSetting
 *
 * validates the end icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateEndIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateCalculationIconSetting
 *
 * validates the calculation icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateCalculationIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  //check formula
  if (!cell.iconSetting.formula) {
    valid = false;
  }

  //check variable
  if (!cell.iconSetting.valueName) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateLoopIconSetting
 *
 * validates the loop icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateLoopIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateTaskIconSetting
 *
 * validates the task icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateTaskIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (!cell.iconSetting.taskJobnetId) {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

/**
 * Function: validateInfoIconSetting
 *
 * validates the info icon setting data.
 *
 * Parameters:
 *
 * showMessage - Boolean that specifies the alert error message must be displayed detail or not.
 *
 * cell - Optional <mxCell> that represents the validate icon setting data.
 *
 * Return true if the icon setting data is valid.
 */
function validateInfoIconSetting(t, cell = null) {
  var valid = true;
  var regex = new RegExp("[^A-Za-z0-9._.-]");

  if (!cell.jobId) {
    valid = false;
  } else {
    if (regex.test(cell.jobId)) {
      valid = false;
    }
  }

  if (cell.iconSetting.infoFlag === 0) {
    if (cell.iconSetting.getJobId) {
      if (cell.hasOwnProperty("parent")) {
        var checkJobIdExist = false;

        for (var i = 0; i < cell.parent.children.length; i++) {
          if (cell.iconSetting.getJobId === cell.parent.children[i].jobId) {
            checkJobIdExist = true;
            break;
          }
        }

        if (!checkJobIdExist) {
          valid = false;
        }
      }
    } else {
      valid = false;
    }
  }else if (cell.iconSetting.infoFlag === 3) {
    if (!cell.iconSetting.getCalendarId) {
      valid = false;
    }
  }else {
    valid = false;
  }

  if (!valid) {
    validateAlertMsg(t, cell);
  }

  return valid;
}

export function validateVertex(t, cell) {
  if (cell.cellType === ICON_TYPE.JOB) {
    if (!validateJobIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.END) {
    if (!validateEndIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.CONDITIONAL_END) {
    if (!validateEndIfIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.CONDITIONAL_START) {
    if (!validateIfIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.PARALLEL_START) {
    if (!validateParallelStartIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.PARALLEL_END) {
    if (!validateParallelEndIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.JOB_CONTROL_VARIABLE) {
    if (!validateJobConVarIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.EXTENDED_JOB) {
    if (!validateExtendedIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.CALCULATION) {
    if (!validateCalculationIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.LOOP) {
    if (!validateLoopIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.TASK) {
    if (!validateTaskIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.INFO) {
    if (!validateInfoIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.FILE_COPY) {
    if (!validateFileTransferIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.FILE_WAIT) {
    if (!validateFileWaitIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.REBOOT) {
    if (!validateRebootIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.RELEASE) {
    if (!validateReleaseIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.AGENT_LESS) {
    if (!validateAgentLessIconSetting(t, cell)) {
      return false;
    }
  } else if (cell.cellType === ICON_TYPE.ZABBIX) {
    if (!validateZabbixIconSetting(t, cell)) {
      return false;
    }
  }
  return true;
}

export function validateIsIdAlreadyExist(value, id, graph) {
  var parent = graph.getDefaultParent();
  if (parent.children != null && parent.children.length > 0) {
    for (var i = 0; i < parent.children.length; i++) {
      if (!parent.children[i].edge) {
        if (
          value === parent.children[i].jobId &&
          id !== parent.children[i].id
        ) {
          return true;
        }
      }
    }
    return false;
  }
}

export function isHankaku(strValue) {
  var num = getShiftJISByteLength(strValue);
  return num === strValue.length;
}

export function isImpossibleStr(strValue) {
  if (strValue.includes('"')) {
    return true;
  }
  if (strValue.includes("'")) {
    return true;
  }
  if (strValue.includes(",")) {
    return true;
  }
  if (strValue.includes("\\")) {
    return true;
  }
  return false;
}

export function isHankakuStrAndUnderbarAndFirstNotNum(strValue) {
  var regexUnderBar = new RegExp(REGEX_PATTERM.MATCH_HANKAKU_UNDERBAR);
  var regexNum = new RegExp(REGEX_PATTERM.MATCH_HANKAKU);
  if (regexUnderBar.test(strValue) && !regexNum.test(strValue[0])) {
    return true;
  }
  return false;
}

export function checkIdOnSelfJobNetwork(value, graph) {
  var parent = graph.getDefaultParent();
  if (parent.children != null && parent.children.length > 0) {
    for (var i = 0; i < parent.children.length; i++) {
      if (!parent.children[i].edge) {
        if (
          value === parent.children[i].jobId
        ) {
          return [parent.children[i], true];
        }
      }
    }
    return [null, false];
  }
}

export function validateGraphData(editorUi, t) {
  var parent = editorUi.editor.graph.getDefaultParent();
  var cell = [];
  var flow = [];
  if (parent.children != null && parent.children.length > 0) {
    for (var i = 0; i < parent.children.length; i++) {
      if (parent.children[i].edge) {
        var geo = parent.children[i].geometry;
        var points = null;
        if (geo.hasOwnProperty("points")) {
          points = parent.children[i].geometry.points;
        }

        var flowStyle = {
          style: parent.children[i].style,
          points: points,
        };
        var flowItem = {
          flowType: parent.children[i].edgeType,
          startJobId: parent.children[i].source.jobId,
          endJobId: parent.children[i].target.jobId,
          flowStyle: JSON.stringify(flowStyle),
        };
        flow.push(flowItem);
      } else {
        if (!validateVertex(t, parent.children[i])) {
          return [false];
        }

        var cellItem = {
          iconType: parent.children[i].cellType,
          iconSetting: parent.children[i].iconSetting,
          x: parent.children[i].geometry.x,
          y: parent.children[i].geometry.y,
          methodFlag: parent.children[i].methodFlag,
        };
        cell.push(cellItem);
      }
    }
  } else {
    alertError(t("title-error"), t("err-no-job"));
    return [false];
  }
  return [true, cell, flow];
}
