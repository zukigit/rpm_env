

function addIconsMapByTypeAndId(type, id, graph, linkJobnetId = null) {
  if (type == jaConstants.ICON_TYPE_JOBNET) {
    if (graph.iconTypeAndIdMap.has(linkJobnetId)) {
      var idArray = graph.iconTypeAndIdMap.get(linkJobnetId);
      if (!idArray.includes(id)) {
        idArray.push(id);
        graph.iconTypeAndIdMap.set(linkJobnetId, idArray);
      }
    } else {
      graph.iconTypeAndIdMap.set(linkJobnetId, [id]);
    }
  } else {
    if (graph.iconTypeAndIdMap.has(type)) {
      var idArray = graph.iconTypeAndIdMap.get(type);
      if (!idArray.includes(id)) {
        idArray.push(id);
        graph.iconTypeAndIdMap.set(type, idArray);
      }
    } else {
      graph.iconTypeAndIdMap.set(type, [id]);
    }
  }
}

function removeIconsMapByTypeAndId(type, id, graph, linkJobnetId) {
  if (graph.graphScreen != jaConstants.JOBNET_EXEC_GRAPH) {
    if (type == jaConstants.ICON_TYPE_JOBNET) {
      if (graph.iconTypeAndIdMap.has(linkJobnetId)) {
        var idArray = graph.iconTypeAndIdMap.get(linkJobnetId);
        idArray = idArray.filter(function (ele) {
          return ele != id;
        });
        graph.iconTypeAndIdMap.set(linkJobnetId, idArray);
      }
    } else {
      if (graph.iconTypeAndIdMap.has(type)) {
        var idArray = graph.iconTypeAndIdMap.get(type);
        idArray = idArray.filter(function (ele) {
          return ele != id;
        });
        graph.iconTypeAndIdMap.set(type, idArray);
      }
    }
  }
}

function checkIconsMapByTypeAndId(type, id, graph, linkJobnetId = null) {
  if (type == jaConstants.ICON_TYPE_JOBNET) {
    return (
      graph.iconTypeAndIdMap.has(linkJobnetId) &&
      graph.iconTypeAndIdMap.get(linkJobnetId).includes(id)
    );
  }
  return (
    graph.iconTypeAndIdMap.has(type) &&
    graph.iconTypeAndIdMap.get(type).includes(id)
  );
}

function reinitializeJobId(graphScreen) {
  iconTypeAndIdMap[graphScreen] = new Map();
  jobnetIdListObj[graphScreen] = JSON.parse(
    JSON.stringify(jaConstants.JOB_ID_LIST)
  );
}

function increaseIconId(cellType, id, graph, iconSetting) {
  if (graph.graphScreen != jaConstants.JOBNET_EXEC_GRAPH) {

    if (cellType == jaConstants.ICON_TYPE_JOBNET) {
      addIconsMapByTypeAndId(cellType, id, graph, iconSetting.linkJobnetId)
    }else{
      addIconsMapByTypeAndId(cellType, id, graph)
    }
    
  }
}

function checkIconIdAndIncrease(cell, graph) {
  var id = null;
  var count = "";
  var linkJobnetId = null;
  if (cell.cellType == jaConstants.ICON_TYPE_JOBNET) {
    id = cell.jobId;
    countStr = "";
    if (graph.jobnetIdListObj[cell.iconSetting.linkJobnetId]) {
      ++graph.jobnetIdListObj[cell.iconSetting.linkJobnetId].count;
      count = graph.jobnetIdListObj[cell.iconSetting.linkJobnetId].count;
      countStr = "-" + count;
    } else {
      graph.jobnetIdListObj[id] = {...jaConstants.JOBNET_ID_INIT_OBJECT};
    }
    while (
      checkIconsMapByTypeAndId(
        cell.cellType,
        cell.iconSetting.linkJobnetId + countStr,
        graph,
        cell.iconSetting.linkJobnetId
      )
    ) {
      count++;
      countStr = "-" + count;
    }
    cell.jobId = cell.iconSetting.linkJobnetId + countStr;
    linkJobnetId = cell.iconSetting.linkJobnetId;
  } else {
    id = graph.jobnetIdListObj[cell.cellType].id;
    if (graph.jobnetIdListObj[cell.cellType].count != null) {
      ++graph.jobnetIdListObj[cell.cellType].count;
      count = graph.jobnetIdListObj[cell.cellType].count;
    }
    while (
      checkIconsMapByTypeAndId(cell.cellType, id + "-" + count, graph)
    ) {
      count++;
    }
    cell.jobId = id + (count == "" ? "" : "-" + count);
  }

  if (cell.iconSetting != null) {
    cell.iconSetting.jobId = cell.jobId;
  } else {
    cell.iconSetting = { jobId: cell.jobId };
  }

  addIconsMapByTypeAndId(cell.cellType, cell.jobId, graph, linkJobnetId);

  if (
    cell.cellType != jaConstants.ICON_TYPE_PARALLEL_START &&
    cell.cellType != jaConstants.ICON_TYPE_PARALLEL_END &&
    cell.cellType != jaConstants.ICON_TYPE_LOOP &&
    cell.cellType != jaConstants.ICON_TYPE_CONDITIONAL_END &&
    cell.cellType !== jaConstants.ICON_TYPE_JOBNET
  ) {
    cell.value = cell.jobId;
  } else if (cell.cellType == jaConstants.ICON_TYPE_JOBNET) {
    let label = `<div style="height: 15px; width: 80px; overflow:hidden; white-space: nowrap; text-overflow: ellipsis;">${cell.jobId}</div>`;
    if (cell.jobName) {
      label += `<div style="height:15px; width: 80px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;">${cell.jobName}</div>`;
    }
    cell.value = label;
  }

  return cell;
}
