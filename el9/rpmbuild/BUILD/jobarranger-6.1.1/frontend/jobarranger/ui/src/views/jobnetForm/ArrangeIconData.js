/**
 * Job Arranger Manager
 * Copyright (C) 2023 Daiwa Institute of Research Ltd. All Rights Reserved.
 * 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements. See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to you under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

/**
 * File : arrange_icon_data
 * 
 * This script is used to validate the icon setting of the jobnet.
 *
 */

import i18next from "i18next";
import { ICON_TYPE } from "../../constants/index";
/**
 * Function: resetJobIconTooltip
 * 
 * prepare tooltip label for job icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetJobIconTooltip(cell, t){
    var jobIconHostName = "";
    if(cell.iconSetting.hostFlag == 0){
        jobIconHostName = "<div>"+t("label-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-host-name")+" : "+cell.iconSetting.hostName+"</div>";
    }else if(cell.iconSetting.hostFlag == 1){
        jobIconHostName = "<div>"+t("label-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-variable")+" : "+cell.iconSetting.hostName+"</div>";
    }
    
    var jobIconStopCommand = cell.iconSetting.stopFlag == 0 ? t("txt-stop-cmd") + " : OFF" : t("txt-stop-cmd") + " : " + cell.iconSetting.stopCommand;
    
    var jobIconForceRun = cell.iconSetting.forceFlag ? "ON" : "OFF";
    var jobIconContinue = cell.iconSetting.continueFlag ? "ON" : "OFF";
    
    var jobIconValueJob = "";
    if(cell.iconSetting.valueJob.length > 0){
        for(var i = 0; i < cell.iconSetting.valueJob.length; i++){
            jobIconValueJob = jobIconValueJob + "<div>&nbsp;&nbsp;&nbsp;"+cell.iconSetting.valueJob[i].valueName+"="+cell.iconSetting.valueJob[i].value+"</div>";
        };
    }
    
    var jobIconValueJobCon = "";
    if(cell.iconSetting.valueJobCon.length > 0){
        for(var i = 0; i < cell.iconSetting.valueJobCon.length; i++){
            jobIconValueJobCon = jobIconValueJobCon + "<div>&nbsp;&nbsp;&nbsp;"+cell.iconSetting.valueJobCon[i]+"</div>";
        };
    }
    var pass=cell.iconSetting.runUserPassword;
    var enPassword="";
    if(pass){
        for(var i=0;i<pass.length;i++){
            enPassword+="*";
        }
    }
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div>"+ jobIconHostName + "<div>"+ jobIconStopCommand + "</div><div>" + t("txt-exec")+" : </div><div style='margin-left: 11px;'>"+ cell.iconSetting.exec + "</div><div>"+t("txt-job-var")+" : </div><div>"+ jobIconValueJob +"</div><div>"+t("label-control-var")+"</div><div>"+ jobIconValueJobCon +"</div><div>"+t("lab-timeout") + cell.iconSetting.timeout +"</div><div>"+t("btn-force-run")+" : "+jobIconForceRun+"</div><div>"+t("txt-ctn1")+" : "+jobIconContinue+"</div><div>"+t("txt-job-stop-code")+" : "+cell.iconSetting.stopCode+ "</div><div>"+t("txt-run-usr")+" : "+ cell.iconSetting.runUser + "</div><div>"+t("txt-pwd")+" : "+ enPassword + "</div>";
}

/**
 * Function: resetEndIconTooltip
 * 
 * prepare tooltip label for end icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
 export function resetEndIconTooltip(cell, t){
    var endIconStopFlag = cell.iconSetting.stopFlag ? "ON" : "OFF";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div> <div>" + t("txt-jobnet-stop") + " : "+ endIconStopFlag +"</div> <div>"+t("label-end-code")+ cell.iconSetting.stopCode +"</div>";
}

/**
 * Function: resetIfIconTooltip
 * 
 * prepare tooltip label for conditional start icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetIfIconTooltip(cell, t){
    var ifIconHandFlag = cell.iconSetting.handFlag ? "Character String" : "Numeric value";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div> <div>"+t("label-var")+ cell.iconSetting.variable +"</div> <div>"+t("txt-proc-mtd")+" : "+ ifIconHandFlag + "</div><div>"+t("txt-comp-val")+" : " + cell.iconSetting.comparisonValue+ "</div>";
}

/**
 * Function: resetJobConVarIconTooltip
 * 
 * prepare tooltip label for conditional job control variable icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetJobConVarIconTooltip(cell, t){

    var jobConVarMsg = "";
    if(cell.iconSetting.valueJob.length > 0){

        for(var i = 0; i < cell.iconSetting.valueJob.length; i++){
            jobConVarMsg = jobConVarMsg + "<div>&nbsp;&nbsp;&nbsp;"+cell.iconSetting.valueJob[i].valueName+"="+cell.iconSetting.valueJob[i].value+"</div>";
        }
    }
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div><div>"+t("label-control-var")+"</div><div>"+ jobConVarMsg +"</div>";

}

/**
 * Function: resetExtendedJobIconTooltip
 * 
 * prepare tooltip label for extended icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetExtendedJobIconTooltip(cell, t){

    var extendedJob = "";
    var extendedJobDesc = "";
    if(cell.iconSetting.commandId.length > 0){
        if(cell.iconSetting.commandId == 'jacmdsleep'){
            extendedJob = t("err-specified-sec");
            extendedJobDesc = t("err-specified-sec-desc");
        }else if(cell.iconSetting.commandId == 'jacmdtime'){
            extendedJob = t("err-specified-time");
            extendedJobDesc = t("err-specified-time-desc");
        }else if(cell.iconSetting.commandId == 'jacmdweek'){
            extendedJob = t("err-check-day");
            extendedJobDesc = t("err-check-day-desc");
        }else if(cell.iconSetting.commandId == 'zabbix_sender'){
            extendedJob = t("exec-zbx-sender");
            extendedJobDesc = t("exec-zbx-sender-desc");
        }
    };

    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div><div>"+t("label-ext-job") + extendedJob +"</div><div>"+ t("txt-par") + " : " + cell.iconSetting.parameter + "</div><div>"+t("label-job-desc")+ extendedJobDesc+"</div>";
}

/**
 * Function: resetCalculationIconTooltip
 * 
 * prepare tooltip label for calculation icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetCalculationIconTooltip(cell, t){
    var calcIconHandFlag = cell.iconSetting.handFlag == 0 ? "Integer calc" : "Time calc";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div><div>"+t("txt-cal-mhd")+ " : " + calcIconHandFlag +"</div><div>"+t("txt-formula") + " : " + cell.iconSetting.formula + "</div><div>"+t("txt-variable") + " : " + cell.iconSetting.valueName+"</div>";
}

/**
 * Function: resetTaskIconTooltip
 * 
 * prepare tooltip label for task icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetTaskIconTooltip(cell, t){
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div><div>"+t("label-jobnet-id")+ " : " + cell.iconSetting.taskJobnetId + "</div><div>"+t("label-jobnet-name")+ " : " + cell.iconSetting.taskJobnetName + "</div>";
}

/**
 * Function: resetInfoIconTooltip
 * 
 * prepare tooltip label for info icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetInfoIconTooltip(cell, t){

    var infoFlagDesc = "";
    if(cell.iconSetting.infoFlag == 0){
        infoFlagDesc = "<div>"+t("txt-info-clfi") + " : " + t("txt-job-sts")+"</div><div>"+t("label-job-info")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("label-job-id")+cell.iconSetting.getJobId+"</div>";
    }else if(cell.iconSetting.infoFlag == 3){
        infoFlagDesc = "<div>"+t("txt-info-clfi") + " : " + t("txt-run-day")+"</div><div>"+t("txt-cal-info")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("label-calendar-id")+ " : " + cell.iconSetting.getCalendarId+"</div><div>&nbsp;&nbsp;&nbsp;"+t("label-calendar-name") + " : " + cell.iconSetting.getCalendarName+"</div>";
        
    }
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div>"+infoFlagDesc;
}

/**
 * Function: resetFileTransferTooltip
 * 
 * prepare tooltip label for file transfer icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetFileTransferTooltip(cell, t){

    var fcopySourceInfo = "";
    if(cell.iconSetting.fromHostFlag == 0){
        fcopySourceInfo = "<div>"+t("txt-src-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-host-name")+ " : " + cell.iconSetting.fromHostName +"</div>";
    }else if(cell.iconSetting.fromHostFlag == 1){
        fcopySourceInfo = "<div>"+t("txt-src-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-variable")+ " : " + cell.iconSetting.fromHostName +"</div>";
    }

    fcopySourceInfo = fcopySourceInfo + "<div>"+t("txt-src-file-info")+"</div><div>&nbsp;&nbsp;&nbsp;" + t("txt-dir") +" : "+ cell.iconSetting.fromDirectory +"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-file-name")+ " : " + cell.iconSetting.fromFileName +"</div>";

    if(cell.iconSetting.toHostFlag == 0){
        fcopySourceInfo = fcopySourceInfo + "<div>"+t("txt-dest-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-host-name") + " : " + cell.iconSetting.toHostName +"</div>";
    }else if(cell.iconSetting.toHostFlag == 1){
        fcopySourceInfo = fcopySourceInfo + "<div>"+t("txt-dest-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-variable")+ " : " + cell.iconSetting.toHostName +"</div>";
    }
    var fcopyIconOverwriteFlag = cell.iconSetting.overwriteFlag == 0 ? "OFF" : "ON";
    
    var fcopyIconForceRun = cell.iconSetting.forceFlag ? "ON" : "OFF";
    fcopySourceInfo = fcopySourceInfo + "<div>"+t("txt-dest-dir-info")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("label-dir")+ cell.iconSetting.toDirectory +"</div><div>&nbsp;&nbsp;&nbsp;"+t("chk-overwrite")+ " : " + fcopyIconOverwriteFlag + "</div><div>"+t("label-force-run")+ fcopyIconForceRun +"</div>";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName + "</div>" + fcopySourceInfo;
}

/**
 * Function: resetFwaitTooltip
 * 
 * prepare tooltip label for file wait icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetFwaitTooltip(cell, t){

    var fwaitIconMsg = "";
    if(cell.iconSetting.hostFlag == 0){
        fwaitIconMsg = "<div>"+t("label-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-host-name") + " : " + cell.iconSetting.hostName +"</div>";
    }else if(cell.iconSetting.hostFlag == 1){
        fwaitIconMsg = "<div>"+t("label-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-variable")+ " : " + cell.iconSetting.hostName +"</div>";
    }
    var fwaitIconModeFlag = cell.iconSetting.fwaitModeFlag == 0 ? t("txt-file-create") : t("txt-chk-file");
    var fwaitIconDeleteFlag = cell.iconSetting.fileDeleteFlag == 0 ? "OFF" : "ON";
    var fwaitIconForceRun = cell.iconSetting.forceFlag ? "ON" : "OFF";
    var fileWaitTime = cell.iconSetting.fwaitModeFlag == 0 ? "<div>" +t("txt-timeout-sec") + " : " + cell.iconSetting.fileWaitTime + "</div>" : "";
    fwaitIconMsg = fwaitIconMsg + "<div>"+t("label-file-name") + cell.iconSetting.fileName + "</div><div>"+t("label-pro-mod")+ fwaitIconModeFlag + "</div><div>"+t("label-del-fil") + fwaitIconDeleteFlag + "</div>"+ fileWaitTime + "<div>"+t("label-force-run")+ fwaitIconForceRun + "</div>";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName + "</div>" + fwaitIconMsg;
}

/**
 * Function: resetRebootTooltip
 * 
 * prepare tooltip label for reboot icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetRebootTooltip(cell, t){
    
    var rebootIconMsg = "";
    if(cell.iconSetting.hostFlag == 0){
        rebootIconMsg = "<div>"+t("label-host")+"</div><div>&nbsp;&nbsp;&nbsp;" + t("txt-host-name") + " : "  + cell.iconSetting.hostName +"</div>";
    }else if(cell.iconSetting.hostFlag == 1){
        rebootIconMsg = "<div>"+t("label-host")+"</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-variable") + " : " + cell.iconSetting.hostName +"</div>";
        
    }
    var rebootIconModeFlag = cell.iconSetting.rebootModeFlag == 0 ? t("txt-force-reboot") : t("txt-reboot-comp-job");
    var rebootIconForceRun = cell.iconSetting.forceFlag ? "ON" : "OFF";
    rebootIconMsg = rebootIconMsg + "</div><div>"+ t("txt-proc-mode") + " : "+ rebootIconModeFlag + "</div><div>"+t("txt-timeout-sec") + " : " + cell.iconSetting.rebootWaitTime + "</div><div>"+t("lab-timeout") + " : " + cell.iconSetting.timeout +"</div><div>"+t("btn-force-run") + " : " + rebootIconForceRun + "</div>";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName + "</div>" + rebootIconMsg;
}

/**
 * Function: resetReleaseTooltip
 * 
 * prepare tooltip label for release icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetReleaseTooltip(cell, t){
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName + "</div><div>"+ t("txt-unhold-job-id") + " : "+ cell.iconSetting.releaseUnholdJobId +"</div>";
}

/**
 * Function: resetAgentLessTooltip
 * 
 * prepare tooltip label for agentless icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetAgentLessTooltip(cell, t){

    var agentLessMsg = "<div>"+t("txt-session")+"</div>";
    if(cell.iconSetting.sessionFlag == 0){
        agentLessMsg += "<div style='margin-left: 11px;'>"+t("txt-one-time")+"</div>";
        if(cell.iconSetting.hostFlag == 0){
            agentLessMsg = agentLessMsg + "<div>"+t("label-host")+"</div><div style='margin-left: 11px;'>"+ t("txt-host-name") + " : "  + cell.iconSetting.hostName +"</div>";
        }else if(cell.iconSetting.hostFlag == 1){
            agentLessMsg = agentLessMsg + "<div>"+t("label-host")+"</div><div style='margin-left: 11px;'>"+t("txt-var")+ " : " + cell.iconSetting.hostName +"</div>";
        }
    }else if(cell.iconSetting.sessionFlag == 1){
        agentLessMsg += "<div style='margin-left: 11px;'>"+t("txt-connect")+"</div><div style='margin-left: 11px;'>"+t("txt-session-id")+ " : "+cell.iconSetting.sessionId+"</div>";
        if(cell.iconSetting.hostFlag == 0){
            agentLessMsg = agentLessMsg + "<div>"+t("label-host")+"</div><div style='margin-left: 11px;'>"+ t("txt-host-name") + " : " + cell.iconSetting.hostName +"</div>";
        }else if(cell.iconSetting.hostFlag == 1){
            agentLessMsg = agentLessMsg + "<div>"+t("label-host")+"</div><div style='margin-left: 11px;'>"+ t("txt-var")+ " : " + cell.iconSetting.hostName +"</div>";
        }
    }else if(cell.iconSetting.sessionFlag == 2){
        agentLessMsg += "<div style='margin-left: 11px;'>"+t("txt-ctn2")+"</div><div style='margin-left: 11px;'"+t("txt-session-id")+ " : "+cell.iconSetting.sessionId+"</div>";
    }else if(cell.iconSetting.sessionFlag == 3){
        agentLessMsg += "<div style='margin-left: 11px;'>"+t("txt-disconnect")+"</div><div style='margin-left: 11px;'>"+t("txt-session-id")+ " : "+cell.iconSetting.sessionId+"</div>";
    }
    var agentLessAuthMethod = "";
    if(cell.iconSetting.authMethod == 0){
        agentLessAuthMethod = t("txt-pwd");

    }else{
        agentLessAuthMethod = t("txt-pub-key");
    }

    var agentLessMode = "";
    if(cell.iconSetting.runMode == 0){
        agentLessMode = t("txt-interact");

    }else{
        agentLessMode = t("txt-not-interact");
    }
    agentLessMsg = agentLessMsg + "<div>SSH</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-auth")+" : "+ agentLessAuthMethod + "</div><div>&nbsp;&nbsp;&nbsp;"+t("txt-mode")+" : " + agentLessMode + "</div>";
    
    if(cell.iconSetting.authMethod == 0){
        var agentPass=cell.iconSetting.loginPassword ;
        var enAgentPass="";
        for(var i=0;i<agentPass.length;i++){
            enAgentPass+="*";
        }
        agentLessMsg = agentLessMsg + "<div>&nbsp;&nbsp;&nbsp;"+t("lab-user-name")+" : "+ cell.iconSetting.loginUser + "<div>&nbsp;&nbsp;&nbsp;"+t("txt-pwd")+" : "+ enAgentPass+"</div>";

    }else{
        agentLessMsg = agentLessMsg +"Public key";
        agentLessMsg = agentLessMsg + "<div>&nbsp;&nbsp;&nbsp;"+t("lab-user-name")+" : "+ cell.iconSetting.loginUser + "<div>&nbsp;&nbsp;&nbsp;"+t("txt-pub-key")+" : "+ cell.iconSetting.publicKey + "<div>&nbsp;&nbsp;&nbsp;"+t("txt-prv-key")+" : "+ cell.iconSetting.privateKey +"<div>&nbsp;&nbsp;&nbsp;"+t("txt-passpharse")+" : "+ cell.iconSetting.passPhrase +"</div>";
    }

    agentLessMsg = agentLessMsg + "<div>"+t("txt-exec")+"</div><div style='margin-left:11px'>" + cell.iconSetting.command + "</div><div>"+t("txt-prompt")+"</div><div>&nbsp;&nbsp;&nbsp;"+ cell.iconSetting.promptString + "</div><div>"+t("txt-char-code")+" : "+ cell.iconSetting.characterCode + "</div>";

    if(cell.iconSetting.lineFeedCode == 0){
        agentLessMsg = agentLessMsg + "<div>"+t("txt-lfc")+" : LF</div>";
    }else if(cell.iconSetting.lineFeedCode == 1){
        agentLessMsg = agentLessMsg + "<div>"+t("txt-lfc")+" : CR</div>";
    }else if(cell.iconSetting.lineFeedCode == 2){
        agentLessMsg = agentLessMsg + "<div>"+t("txt-lfc")+" : CRLF</div>";
    }
    var agentLessIconForceRun = cell.iconSetting.forceFlag ? "ON" : "OFF";
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div>"+ agentLessMsg +"</div><div>"+t("txt-timeout-min")+" : "+ cell.iconSetting.timeout + "</div><div>"+ t("txt-job-stop-code") + " : "+ cell.iconSetting.stopCode + "</div><div>"+t("btn-force-run")+" : "+ agentLessIconForceRun +"</div>";
}

/**
 * Function: resetZabbixTooltip
 * 
 * prepare tooltip label for zabbix icon.
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetZabbixTooltip(cell, t){

    var zabbixMsg = "<div>" + t("txt-link-trg");
    if(cell.iconSetting.linkTarget == 0){
        zabbixMsg = zabbixMsg + " : "+ t("txt-host-gp") + "</div>";
    }else if(cell.iconSetting.linkTarget == 1){
        zabbixMsg = zabbixMsg + " : "+ t("txt-host-name") + "</div>";
    }else if(cell.iconSetting.linkTarget == 2){
        zabbixMsg = zabbixMsg + " : "+ t("txt-item") + "</div>";
    }else if(cell.iconSetting.linkTarget == 3){
        zabbixMsg = zabbixMsg + " : "+ t("txt-trigger") + "</div>";
    }
    zabbixMsg = zabbixMsg + "<div>" + t("txt-link-mode");
    if(cell.iconSetting.linkOperation == 0){
        zabbixMsg = zabbixMsg + " : "+ t("col-enabled") + "</div>";
    }else if(cell.iconSetting.linkOperation == 1){
        zabbixMsg = zabbixMsg + " : "+ t("col-disable") + "</div>";
    }else if(cell.iconSetting.linkOperation == 2){
        zabbixMsg = zabbixMsg + " : "+ t("txt-get-sts") + "</div>";
    }else if(cell.iconSetting.linkOperation == 3){
        zabbixMsg = zabbixMsg + " : "+ t("txt-get-data") + "</div>";
    }
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div>"+zabbixMsg;
}

/**
 * Function: resetDefaultTooltip
 * 
 * prepare default tooltip label
 * 
 * Parameters:
 * 
 * cell - <mxCell> whose icon's tooltip data should be reset.
 * 
 * Return prepare tooltip string.
 */
export function resetDefaultTooltip(cell, t){
    return "<div>"+t("label-job-id")+ cell.iconSetting.jobId +"</div> <div>"+t("label-job-name")+ cell.iconSetting.jobName +"</div>";
}

/**
 * Function: arranageIconSetting
 * 
 * prepare response object from server to iconsetting object that is compatible with <mxCell> object's iconsetting properties.
 * It's also prepare tooltip label for each icon.
 * 
 * Parameters:
 * 
 * data - object that is response data from server.
 * graph - Reference to the enclosing <mxGraph>.
 * 
 * Return array
 * First item is arranged iconsetting object.
 * Second item is prepare tooltip label.
 */
export function arranageIconSetting(data, graph){
    const t = i18next.t; 
    let iconSetting, toolTipLabel;
    
    if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.JOB){
        let command = "";
        let stopCommand = "";
        for(let i = 0; i < data.jobCommand.length; i++){
            if(data.jobCommand[i].command_cls == 0){
                command = data.jobCommand[i].command;
            }

            if(data.jobCommand[i].command_cls == 2){
                stopCommand = data.jobCommand[i].command;
            }
        }

        let valueJob = [];
        for(let i = 0; i < data.valueJob.length; i++){
            valueJob.push({
                key: i,
                valueName : data.valueJob[i].value_name,
                value : data.valueJob[i].value
            })
        }

        let valueJobCon = [];
        for(let i = 0; i < data.valueJobControl.length; i++){
            valueJobCon.push(data.valueJobControl[i].value_name)
        }
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            hostFlag : data.iconSetting.hostFlag ? parseInt(data.iconSetting.hostFlag) : 0,
            hostName : data.iconSetting.hostName,
            stopFlag : parseInt(data.iconSetting.stopFlag),
            stopCommand : stopCommand ? stopCommand : "",
            exec : command ? command : "",
            valueJob : valueJob,
            valueJobCon : valueJobCon,
            stopCode : data.iconSetting.stopCode ? data.iconSetting.stopCode : "",
            forceFlag : parseInt(data.jobData.force_flag),
            continueFlag : parseInt(data.jobData.continue_flag),
            timeout : data.iconSetting.timeout ? data.iconSetting.timeout : 0,
            timeoutRunType : data.iconSetting.timeoutRunType ? data.iconSetting.timeoutRunType : "",
            runUser : data.jobData.run_user ? data.jobData.run_user : "",
            runUserPassword : data.jobData.run_user_password ? data.jobData.run_user_password : "",
        }

        toolTipLabel = resetJobIconTooltip({iconSetting : iconSetting}, t);
        
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.START){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
        }
        toolTipLabel = resetDefaultTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.END){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            stopFlag : parseInt(data.iconSetting.jobnetStopFlag),
            stopCode : data.iconSetting.jobnetStopCode ? data.iconSetting.jobnetStopCode : "",
        }
        toolTipLabel = resetEndIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.CONDITIONAL_END){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
        }
        toolTipLabel = resetDefaultTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.CONDITIONAL_START){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            variable : data.iconSetting.valueName ? data.iconSetting.valueName : "",
            handFlag : parseInt(data.iconSetting.handFlag),
            comparisonValue : data.iconSetting.comparisonValue ? data.iconSetting.comparisonValue : "",
        }
        toolTipLabel = resetIfIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.PARALLEL_START){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
        }
        toolTipLabel = resetDefaultTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.PARALLEL_END){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
        }
        toolTipLabel = resetDefaultTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.JOB_CONTROL_VARIABLE){
        let valueJobIcon = [];
        for(let i in data.iconSetting){
            valueJobIcon.push({
                key: parseInt(i),
                valueName : data.iconSetting[parseInt(i)].value_name,
                value : data.iconSetting[parseInt(i)].value
            })
        }
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            valueJob : valueJobIcon,
        }
        toolTipLabel = resetJobConVarIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.EXTENDED_JOB){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            commandId : data.iconSetting.commandId ? data.iconSetting.commandId : "",
            parameter : data.iconSetting.value ? data.iconSetting.value : "",
        }
        toolTipLabel = resetExtendedJobIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.CALCULATION){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            handFlag :  parseInt(data.iconSetting.handFlag),
            formula : data.iconSetting.formula ? data.iconSetting.formula : "",
            valueName : data.iconSetting.valueName ? data.iconSetting.valueName : "",
        }
        toolTipLabel = resetCalculationIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.LOOP){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
        }
        toolTipLabel = resetDefaultTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.TASK){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            taskJobnetId : data.iconSetting.submitJobnetId ? data.iconSetting.submitJobnetId : "",
            taskJobnetName : data.iconSetting.jobnetName ? data.iconSetting.jobnetName : ""
        }
        toolTipLabel = resetTaskIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.INFO){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            infoFlag : data.iconSetting.hasOwnProperty("infoFlag") ? parseInt(data.iconSetting.infoFlag) : 0,
            getJobId : data.iconSetting.getJobId ? data.iconSetting.getJobId : "",
            getCalendarId : data.iconSetting.getCalendarId ? data.iconSetting.getCalendarId : "",
            getCalendarName : data.iconSetting.calendarName ? data.iconSetting.calendarName : ""
        }
        toolTipLabel = resetInfoIconTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.FILE_COPY){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            fromHostFlag : data.iconSetting.fromHostFlag ? parseInt(data.iconSetting.fromHostFlag) : 0,
            toHostFlag : data.iconSetting.toHostFlag ? parseInt(data.iconSetting.toHostFlag) : 0,
            fromHostName : data.iconSetting.fromHostName ? data.iconSetting.fromHostName : "",
            toHostName : data.iconSetting.toHostName ? data.iconSetting.toHostName : "",
            overwriteFlag : parseInt(data.iconSetting.overwriteFlag),
            forceFlag : parseInt(data.jobData.force_flag),
            fromDirectory : data.iconSetting.fromDirectory ? data.iconSetting.fromDirectory : "",
            toDirectory : data.iconSetting.toDirectory ? data.iconSetting.toDirectory : "",
            fromFileName : data.iconSetting.fromFileName ? data.iconSetting.fromFileName : "",
        }
        toolTipLabel = resetFileTransferTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.FILE_WAIT){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            hostFlag : data.iconSetting.hostFlag ? parseInt(data.iconSetting.hostFlag) : 0,
            hostName : data.iconSetting.hostName ? data.iconSetting.hostName : "",
            fileName : data.iconSetting.fileName ? data.iconSetting.fileName : "",
            continueFlag : parseInt(data.jobData.continue_flag),
            forceFlag : parseInt(data.jobData.force_flag),
            fwaitModeFlag : parseInt(data.iconSetting.fwaitModeFlag),
            fileDeleteFlag : parseInt(data.iconSetting.fileDeleteFlag),
            fileWaitTime : data.iconSetting.fileWaitTime ? data.iconSetting.fileWaitTime : "",
        }
        toolTipLabel = resetFwaitTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.REBOOT){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            hostFlag : data.iconSetting.hostFlag ? parseInt(data.iconSetting.hostFlag) : 0,
            hostName : data.iconSetting.hostName ? data.iconSetting.hostName : "",
            rebootWaitTime : data.iconSetting.rebootWaitTime ? data.iconSetting.rebootWaitTime : "",
            forceFlag : parseInt(data.jobData.force_flag),
            rebootModeFlag : parseInt(data.iconSetting.rebootModeFlag),
            timeout : parseInt(data.iconSetting.timeout),
        }
        toolTipLabel = resetRebootTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.RELEASE){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            releaseUnholdJobId : data.iconSetting.releaseJobId ? data.iconSetting.releaseJobId : "",
        }
        toolTipLabel = resetReleaseTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.AGENT_LESS){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            hostFlag : data.iconSetting.hostFlag ? parseInt(data.iconSetting.hostFlag) : 0,
            connectionMethod : data.iconSetting.hasOwnProperty("connectionMethod") ? parseInt(data.iconSetting.connectionMethod) : 0,
            sessionFlag : data.iconSetting.hasOwnProperty("sessionFlag") ? parseInt(data.iconSetting.sessionFlag) : 0,
            authMethod : data.iconSetting.hasOwnProperty("authMethod") ? parseInt(data.iconSetting.authMethod) : 0,
            runMode : data.iconSetting.hasOwnProperty("runMode") ? parseInt(data.iconSetting.runMode) : 0,
            lineFeedCode : data.iconSetting.hasOwnProperty("lineFeedCode") ? parseInt(data.iconSetting.lineFeedCode) : 0,
            timeout : data.iconSetting.hasOwnProperty("timeout") ? parseInt(data.iconSetting.timeout) : 0,
            sessionId : data.iconSetting.sessionId ? data.iconSetting.sessionId : "",
            loginUser : data.iconSetting.loginUser ? data.iconSetting.loginUser : "",
            loginPassword : data.iconSetting.loginPassword ? data.iconSetting.loginPassword : "",
            publicKey : data.iconSetting.publicKey ? data.iconSetting.publicKey : "",
            privateKey : data.iconSetting.privateKey ? data.iconSetting.privateKey : "",
            passPhrase : data.iconSetting.passphrase ? data.iconSetting.passphrase : "",
            hostName : data.iconSetting.hostName ? data.iconSetting.hostName : "",
            stopCode : data.iconSetting.stopCode ? data.iconSetting.stopCode : "",
            terminalType : data.iconSetting.terminalType ? data.iconSetting.terminalType : "",
            characterCode : data.iconSetting.characterCode ? data.iconSetting.characterCode : "",
            promptString : data.iconSetting.promptString ? data.iconSetting.promptString : "",
            command : data.iconSetting.command ? data.iconSetting.command : "",
            forceFlag : data.jobData.hasOwnProperty("force_flag") ? parseInt(data.jobData.force_flag) : 0
        }
        toolTipLabel = resetAgentLessTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.ZABBIX){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name ? data.jobData.job_name : "",
            linkTarget : parseInt(data.iconSetting.linkTarget),
            linkOperation : parseInt(data.iconSetting.linkOperation),
            groupId : parseInt(data.iconSetting.groupid),
            hostId : parseInt(data.iconSetting.hostid),
            itemId : parseInt(data.iconSetting.itemid),
            triggerId : parseInt(data.iconSetting.triggerid),
        }
        toolTipLabel = resetZabbixTooltip({iconSetting : iconSetting}, t);
    }else if(graph.jobTypeList[data.jobData.job_type].cellType == ICON_TYPE.JOBNET){
        iconSetting = {
            jobId : data.jobData.job_id,
            jobName : data.jobData.job_name,
            linkJobnetId : data.iconSetting.linkJobnetId,
        }
        if(data.iconSetting.hasOwnProperty("linkInnerJobnetId")){
            iconSetting["linkInnerJobnetId"] = data.iconSetting.linkInnerJobnetId;
        }
        toolTipLabel = resetDefaultTooltip({iconSetting : iconSetting}, t);
    }

    return [iconSetting, toolTipLabel];
}