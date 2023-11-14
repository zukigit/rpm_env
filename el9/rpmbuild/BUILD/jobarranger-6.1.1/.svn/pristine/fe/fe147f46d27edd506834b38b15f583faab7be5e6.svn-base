/**
 * Copyright (c) 2006-2015, JGraph Ltd
 * Copyright (c) 2006-2015, Gaudenz Alder
 */
/**
 * Class: mxCell
 *
 * Cells are the elements of the graph model. They represent the state
 * of the groups, vertices and edges in a graph.
 * 
 * Custom attributes:
 * 
 * For custom attributes we recommend using an XML node as the value of a cell.
 * The following code can be used to create a cell with an XML node as the
 * value:
 * 
 * (code)
 * var doc = mxUtils.createXmlDocument();
 * var node = doc.createElement('MyNode')
 * node.setAttribute('label', 'MyLabel');
 * node.setAttribute('attribute1', 'value1');
 * graph.insertVertex(graph.getDefaultParent(), null, node, 40, 40, 80, 30);
 * (end)
 * 
 * For the label to work, <mxGraph.convertValueToString> and
 * <mxGraph.cellLabelChanged> should be overridden as follows:
 * 
 * (code)
 * graph.convertValueToString = function(cell)
 * {
 *   if (mxUtils.isNode(cell.value))
 *   {
 *     return cell.getAttribute('label', '')
 *   }
 * };
 * 
 * var cellLabelChanged = graph.cellLabelChanged;
 * graph.cellLabelChanged = function(cell, newValue, autoSize)
 * {
 *   if (mxUtils.isNode(cell.value))
 *   {
 *     // Clones the value for correct undo/redo
 *     var elt = cell.value.cloneNode(true);
 *     elt.setAttribute('label', newValue);
 *     newValue = elt;
 *   }
 *   
 *   cellLabelChanged.apply(this, arguments);
 * };
 * (end)
 * 
 * Callback: onInit
 *
 * Called from within the constructor.
 * 
 * Constructor: mxCell
 *
 * Constructs a new cell to be used in a graph model.
 * This method invokes <onInit> upon completion.
 * 
 * Parameters:
 * 
 * value - Optional object that represents the cell value.
 * geometry - Optional <mxGeometry> that specifies the geometry.
 * style - Optional formatted string that defines the style.
 */
 function mxCell(value, geometry, style, cellType, dValue, jobId, jobName)
 {
	 this.dValue = dValue;
	 this.cellType = cellType;
	 this.jobId = jobId;
	 this.jobName = jobName;
	 this.value = value;
	 this.setGeometry(geometry);
	 this.setStyle(style);
	 
	 if (this.onInit != null)
	 {
		 this.onInit();
	 }
 };
 
 /**
  * Variable: id
  *
  * Holds the Id. Default is null.
  */
 mxCell.prototype.id = null;
 
 /**
  * Variable: value
  *
  * Holds the user object. Default is null.
  */
 mxCell.prototype.value = null;
 
 /**
  * Variable: geometry
  *
  * Holds the <mxGeometry>. Default is null.
  */
 mxCell.prototype.geometry = null;
 
 /**
  * Variable: style
  *
  * Holds the style as a string of the form [(stylename|key=value);]. Default is
  * null.
  */
 mxCell.prototype.style = null;
 
 /**
  * Variable: cellType
  *
  * Holds the cellType. Default is null.
  */
  mxCell.prototype.cellType = null;
 
  /**
  * Variable: dValue
  *
  * Holds the dValue for dialog. Default is null.
  */
   mxCell.prototype.dValue = null;
 
   /**
  * Variable: jobId
  *
  * Job id of icon setting
  */
	mxCell.prototype.jobId = null;
 
 /**
  * Variable: jobName
  *
  * job name of icon setting
  */
   mxCell.prototype.jobName = null;
 
   /**
  * Variable: iconSetting
  *
  * detail info of icon setting
  */
	mxCell.prototype.iconSetting = null;

     /**
  * Variable: incomeingEdge
  *
  * check icon have already incoming edge
  */
  mxCell.prototype.incomingEdge = {
    normalFlow : 0,
    trueFlow : 0,
    falseFlow : 0
  } //Property will be edge type and Value will be edge count;

  /**
  * Variable: outgoingEdge
  *
  * check icon have already outgoing edge
  */
  mxCell.prototype.outgoingEdge = {
    normalFlow : 0,
    trueFlow : 0,
    falseFlow : 0
  } //Property will be edge type and Value will be edge count;

/**
* Variable: toolTipLabel
*
* tooltip label displayed when jobnet execution
*/
mxCell.prototype.toolTipLabel = '';

/**
* Variable: latestVertexData
*
* latest vertex data when jobnet execution
*/
mxCell.prototype.latestVertexData;

/**
* Variable: beforeVariables
*
* before variable list displayed when jobnet execution
*/
mxCell.prototype.beforeVariables;

/**
* Variable: afterVariables
*
* after variable list displayed when jobnet execution
*/
mxCell.prototype.afterVariables;

/**
* Variable: jobnetRunStatus
*
* status of the jobnet is displayed when jobnet execution
*/
mxCell.prototype.jobnetRunStatus = null;

/**
* Variable: runStatus
*
* status of the job is displayed when jobnet execution
*/
mxCell.prototype.runStatus = null;

/**
* Variable: innerJobnetMainId
*
* unique id of main run jobnet is displayed when jobnet execution
*/
mxCell.prototype.innerJobnetMainId;

/**
* Variable: innerJobnetId
*
* unique id of run jobnet is displayed when jobnet execution
*/
mxCell.prototype.innerJobnetId;

/**
* Variable: innerJobId
*
* unique id of run job is displayed when jobnet execution
*/
mxCell.prototype.innerJobId;

/**
* Variable: methodFlag
*
* method flag is displayed when jobnet execution
*/
mxCell.prototype.methodFlag = 0;

/**
* Variable: jobnetAbortFlag
*
* jobnet abort flag is displayed when jobnet execution
*/
mxCell.prototype.jobnetAbortFlag = null;

  /**
* Variable: edgeType
*
* check edgeType => 0 : normal flow, 1 : true flow, 2 : false
*/
mxCell.prototype.edgeType = null;
 
 /**
  * Variable: vertex
  *
  * Specifies whether the cell is a vertex. Default is false.
  */
 mxCell.prototype.vertex = false;
 
 /**
  * Variable: edge
  *
  * Specifies whether the cell is an edge. Default is false.
  */
 mxCell.prototype.edge = false;
 
 /**
  * Variable: connectable
  *
  * Specifies whether the cell is connectable. Default is true.
  */
 mxCell.prototype.connectable = true;
 
 /**
  * Variable: visible
  *
  * Specifies whether the cell is visible. Default is true.
  */
 mxCell.prototype.visible = true;
 
 /**
  * Variable: collapsed
  *
  * Specifies whether the cell is collapsed. Default is false.
  */
 mxCell.prototype.collapsed = false;
 
 /**
  * Variable: parent
  *
  * Reference to the parent cell.
  */
 mxCell.prototype.parent = null;
 
 /**
  * Variable: source
  *
  * Reference to the source terminal.
  */
 mxCell.prototype.source = null;
 
 /**
  * Variable: target
  *
  * Reference to the target terminal.
  */
 mxCell.prototype.target = null;

  /**
  * Variable: sourceId
  *
  * Reference to the source terminal id.
  */
   mxCell.prototype.sourceId = null;
 
   /**
    * Variable: targetId
    *
    * Reference to the target terminal id.
    */
   mxCell.prototype.targetId = null;
 
 /**
  * Variable: children
  *
  * Holds the child cells.
  */
 mxCell.prototype.children = null;
 
 /**
  * Variable: edges
  *
  * Holds the edges.
  */
 mxCell.prototype.edges = null;
 
 /**
  * Variable: mxTransient
  *
  * List of members that should not be cloned inside <clone>. This field is
  * passed to <mxUtils.clone> and is not made persistent in <mxCellCodec>.
  * This is not a convention for all classes, it is only used in this class
  * to mark transient fields since transient modifiers are not supported by
  * the language.
  */
 mxCell.prototype.mxTransient = ['id', 'value', 'parent', 'source',
								 'target', 'children', 'edges'];
 
 /**
  * Function: getId
  *
  * Returns the Id of the cell as a string.
  */
 mxCell.prototype.getId = function()
 {
	 return this.id;
 };
		 
 /**
  * Function: setId
  *
  * Sets the Id of the cell to the given string.
  */
 mxCell.prototype.setId = function(id)
 {
	 this.id = id;
 };
 
 /**
  * Function: getValue
  *
  * Returns the user object of the cell. The user
  * object is stored in <value>.
  */
 mxCell.prototype.getValue = function()
 {
	 return this.value;
 };
		 
 /**
  * Function: setValue
  *
  * Sets the user object of the cell. The user object
  * is stored in <value>.
  */
 mxCell.prototype.setValue = function(value)
 {
	 this.value = value;
 };
 
 /**
  * Function: valueChanged
  *
  * Changes the user object after an in-place edit
  * and returns the previous value. This implementation
  * replaces the user object with the given value and
  * returns the old user object.
  */
 mxCell.prototype.valueChanged = function(newValue)
 {
	 var previous = this.getValue();
	 this.setValue(newValue);
	 
	 return previous;
 };
 
 /**
  * Function: getGeometry
  *
  * Returns the <mxGeometry> that describes the <geometry>.
  */
 mxCell.prototype.getGeometry = function()
 {
	 return this.geometry;
 };
 
 /**
  * Function: setGeometry
  *
  * Sets the <mxGeometry> to be used as the <geometry>.
  */
 mxCell.prototype.setGeometry = function(geometry)
 {
	 this.geometry = geometry;
 };
 
 /**
  * Function: getStyle
  *
  * Returns a string that describes the <style>.
  */
 mxCell.prototype.getStyle = function()
 {
	 return this.style;
 };
 
 /**
  * Function: setStyle
  *
  * Sets the string to be used as the <style>.
  */
 mxCell.prototype.setStyle = function(style)
 {
	 this.style = style;
 };

 /**
  * Function: getJobId
  *
  * Returns a string that describes the <jobId>.
  */
  mxCell.prototype.getJobId = function()
  {
    return this.jobId;
  };
  
  /**
   * Function: setJobId
   *
   * Sets the string to be used as the <jobId>.
   */
  mxCell.prototype.setJobId = function(jobId)
  {
    this.jobId = jobId;
  };

  /**
  * Function: getJobName
  *
  * Returns a string that describes the <jobName>.
  */
   mxCell.prototype.getJobName = function()
   {
     return this.jobName;
   };
   
   /**
    * Function: setJobnetName
    *
    * Sets the string to be used as the <jobName>.
    */
   mxCell.prototype.setJobName = function(jobName)
   {
     this.jobName = jobName;
   };

   /**
  * Function: getIconSetting
  *
  * Returns a string that describes the <iconSetting>.
  */
    mxCell.prototype.getIconSetting = function()
    {
      return this.iconSetting;
    };
    
    /**
     * Function: setIconSetting
     *
     * Sets the string to be used as the <iconSetting>.
     */
    mxCell.prototype.setIconSetting = function(iconSetting)
    {
      this.iconSetting = iconSetting;
    };

 /**
  * Function: getTooltipLable
  *
  * Returns a string that describes the <tooltipLabel>.
  */
  mxCell.prototype.getTooltipLabel = function()
  {
    return this.toolTipLabel;
  };

  /**
  * Function: setTooltipLable
  *
  * Sets the string to be used as the <tooltipLabel>.
  */
   mxCell.prototype.setTooltipLabel = function(tooltipLabel)
   {
     this.toolTipLabel = tooltipLabel;
   };

 /**
  * Function: getLatestVertexData
  *
  * Returns a string that describes the <latestVertexData>.
  */
  mxCell.prototype.getLatestVertexData = function()
  {
    return this.latestVertexData;
  };

  /**
  * Function: setLatestVertexData
  *
  * Sets the string to be used as the <latestVertexData>.
  */
   mxCell.prototype.setLatestVertexData = function(latestVertexData)
   {
     this.latestVertexData = latestVertexData;
     this.toolTipLabel = latestVertexData.toolTipLabel;
     this.beforeVariables = latestVertexData.beforeVariables;
     this.afterVariables = latestVertexData.afterVariables;
     this.runStatus = latestVertexData.runStatus;
     this.methodFlag = latestVertexData.methodFlag;
     this.jobnetAbortFlag = latestVertexData.jobnetAbortFlag;
     this.jobnetRunStatus = latestVertexData.jobnetRunStatus;
   };

 /**
  * Function: getBeforeVariables
  *
  * Returns a string that describes the <beforeVariables>.
  */
  mxCell.prototype.getBeforeVariables = function()
  {
    return this.beforeVariables;
  };

  /**
  * Function: setBeforeVariables
  *
  * Sets the string to be used as the <beforeVariables>.
  */
   mxCell.prototype.setBeforeVariables = function(beforeVariables)
   {
     this.beforeVariables = beforeVariables;
   };

  /**
    * Function: getAfterVariables
    *
    * Returns a string that describes the <afterVariables>.
    */
  mxCell.prototype.getAfterVariables = function()
  {
    return this.afterVariables;
  };

  /**
   * Function: setAfterVariables
   *
   * Sets the string to be used as the <afterVariables>.
   */
  mxCell.prototype.setAfterVariables = function(afterVariables)
  {
    this.afterVariables = afterVariables;
  };

  /**
    * Function: getRunStatus
    *
    * Returns a string that describes the <runStatus>.
    */
   mxCell.prototype.getRunStatus = function()
   {
     return this.runStatus;
   };
 
   /**
    * Function: setRunStatus
    *
    * Sets the string to be used as the <runStatus>.
    */
   mxCell.prototype.setRunStatus = function(runStatus)
   {
     this.runStatus = runStatus;
   };

  /**
    * Function: getMethodFlag
    *
    * Returns a string that describes the <methodFlag>.
    */
  mxCell.prototype.getMethodFlag = function()
  {
    return this.methodFlag;
  };
 
  /**
  * Function: setMethodFlag
  *
  * Sets the string to be used as the <methodFlag>.
  */
  mxCell.prototype.setMethodFlag = function(methodFlag)
  {
    this.methodFlag = methodFlag;
  };

  /**
    * Function: getJobnetAbortFlag
    *
    * Returns a string that describes the <jobnetAbortFlag>.
    */
   mxCell.prototype.getJobnetAbortFlag = function()
   {
     return this.jobnetAbortFlag;
   };
  
   /**
   * Function: setJobnetAbortFlag
   *
   * Sets the string to be used as the <jobnetAbortFlag>.
   */
   mxCell.prototype.setJobnetAbortFlag = function(jobnetAbortFlag)
   {
     this.jobnetAbortFlag = jobnetAbortFlag;
   };

  /**
    * Function: getJobnetRunStatus
    *
    * Returns a string that describes the <jobnetRunStatus>.
    */
   mxCell.prototype.getJobnetRunStatus = function()
   {
     return this.jobnetRunStatus;
   };
  
   /**
   * Function: setJobnetRunStatus
   *
   * Sets the string to be used as the <jobnetAbortFlag>.
   */
   mxCell.prototype.setJobnetRunStatus = function(jobnetRunStatus)
   {
     this.jobnetRunStatus = jobnetRunStatus;
   };
 
 /**
  * Function: isVertex
  *
  * Returns true if the cell is a vertex.
  */
 mxCell.prototype.isVertex = function()
 {
	 return this.vertex != 0;
 };
 
 /**
  * Function: setVertex
  *
  * Specifies if the cell is a vertex. This should only be assigned at
  * construction of the cell and not be changed during its lifecycle.
  * 
  * Parameters:
  * 
  * vertex - Boolean that specifies if the cell is a vertex.
  */
 mxCell.prototype.setVertex = function(vertex)
 {
	 this.vertex = vertex;
 };
 
 /**
  * Function: isEdge
  *
  * Returns true if the cell is an edge.
  */
 mxCell.prototype.isEdge = function()
 {
	 return this.edge != 0;
 };
	 
 /**
  * Function: setEdge
  * 
  * Specifies if the cell is an edge. This should only be assigned at
  * construction of the cell and not be changed during its lifecycle.
  * 
  * Parameters:
  * 
  * edge - Boolean that specifies if the cell is an edge.
  */
 mxCell.prototype.setEdge = function(edge)
 {
	 this.edge = edge;
 };
 
 /**
  * Function: isConnectable
  *
  * Returns true if the cell is connectable.
  */
 mxCell.prototype.isConnectable = function()
 {
	 return this.connectable != 0;
 };
 
 /**
  * Function: setConnectable
  *
  * Sets the connectable state.
  * 
  * Parameters:
  * 
  * connectable - Boolean that specifies the new connectable state.
  */
 mxCell.prototype.setConnectable = function(connectable)
 {
	 this.connectable = connectable;
 };
 
 /**
  * Function: isVisible
  *
  * Returns true if the cell is visibile.
  */
 mxCell.prototype.isVisible = function()
 {
	 return this.visible != 0;
 };
 
 /**
  * Function: setVisible
  *
  * Specifies if the cell is visible.
  * 
  * Parameters:
  * 
  * visible - Boolean that specifies the new visible state.
  */
 mxCell.prototype.setVisible = function(visible)
 {
	 this.visible = visible;
 };
 
 /**
  * Function: isCollapsed
  *
  * Returns true if the cell is collapsed.
  */
 mxCell.prototype.isCollapsed = function()
 {
	 return this.collapsed != 0;
 };
 
 /**
  * Function: setCollapsed
  *
  * Sets the collapsed state.
  * 
  * Parameters:
  * 
  * collapsed - Boolean that specifies the new collapsed state.
  */
 mxCell.prototype.setCollapsed = function(collapsed)
 {
	 this.collapsed = collapsed;
 };
 
 /**
  * Function: getParent
  *
  * Returns the cell's parent.
  */
 mxCell.prototype.getParent = function()
 {
	 return this.parent;
 };
 
 /**
  * Function: setParent
  *
  * Sets the parent cell.
  * 
  * Parameters:
  * 
  * parent - <mxCell> that represents the new parent.
  */
 mxCell.prototype.setParent = function(parent)
 {
	 this.parent = parent;
 };
 
 /**
  * Function: getTerminal
  *
  * Returns the source or target terminal.
  * 
  * Parameters:
  * 
  * source - Boolean that specifies if the source terminal should be
  * returned.
  */
 mxCell.prototype.getTerminal = function(source)
 {
	 return (source) ? this.source : this.target;
 };
 
 /**
  * Function: setTerminal
  *
  * Sets the source or target terminal and returns the new terminal.
  * 
  * Parameters:
  * 
  * terminal - <mxCell> that represents the new source or target terminal.
  * isSource - Boolean that specifies if the source or target terminal
  * should be set.
  */
 mxCell.prototype.setTerminal = function(terminal, isSource)
 {
	 if (isSource)
	 {
		 this.source = terminal;
	 }
	 else
	 {
		 this.target = terminal;
	 }
	 
	 return terminal;
 };
 
 /**
  * Function: getChildCount
  *
  * Returns the number of child cells.
  */
 mxCell.prototype.getChildCount = function()
 {
	 return (this.children == null) ? 0 : this.children.length;
 };
 
 /**
  * Function: getIndex
  *
  * Returns the index of the specified child in the child array.
  * 
  * Parameters:
  * 
  * child - Child whose index should be returned.
  */
 mxCell.prototype.getIndex = function(child)
 {
	 return mxUtils.indexOf(this.children, child);
 };
 
 /**
  * Function: getChildAt
  *
  * Returns the child at the specified index.
  * 
  * Parameters:
  * 
  * index - Integer that specifies the child to be returned.
  */
 mxCell.prototype.getChildAt = function(index)
 {
	 return (this.children == null) ? null : this.children[index];
 };
 
 /**
  * Function: insert
  *
  * Inserts the specified child into the child array at the specified index
  * and updates the parent reference of the child. If not childIndex is
  * specified then the child is appended to the child array. Returns the
  * inserted child.
  * 
  * Parameters:
  * 
  * child - <mxCell> to be inserted or appended to the child array.
  * index - Optional integer that specifies the index at which the child
  * should be inserted into the child array.
  */
 mxCell.prototype.insert = function(child, index)
 {
	 if (child != null)
	 {
		 if (index == null)
		 {
			 index = this.getChildCount();
			 
			 if (child.getParent() == this)
			 {
				 index--;
			 }
		 }
 
		 child.removeFromParent();
		 child.setParent(this);
		 
		 if (this.children == null)
		 {
			 this.children = [];
			 this.children.push(child);
		 }
		 else
		 {
			 this.children.splice(index, 0, child);
		 }
	 }
	 
	 return child;
 };
 
 /**
  * Function: remove
  *
  * Removes the child at the specified index from the child array and
  * returns the child that was removed. Will remove the parent reference of
  * the child.
  * 
  * Parameters:
  * 
  * index - Integer that specifies the index of the child to be
  * removed.
  */
 mxCell.prototype.remove = function(index)
 {
	 var child = null;
	 
	 if (this.children != null && index >= 0)
	 {
		 child = this.getChildAt(index);
		 
		 if (child != null)
		 {
			 this.children.splice(index, 1);
			 child.setParent(null);
		 }
	 }
	 
	 return child;
 };
 
 /**
  * Function: removeFromParent
  *
  * Removes the cell from its parent.
  */
 mxCell.prototype.removeFromParent = function()
 {
	 if (this.parent != null)
	 {
		 var index = this.parent.getIndex(this);
		 this.parent.remove(index);
	 }
 };
 
 /**
  * Function: getEdgeCount
  *
  * Returns the number of edges in the edge array.
  */
 mxCell.prototype.getEdgeCount = function()
 {
	 return (this.edges == null) ? 0 : this.edges.length;
 };
 
 /**
  * Function: getEdgeIndex
  *
  * Returns the index of the specified edge in <edges>.
  * 
  * Parameters:
  * 
  * edge - <mxCell> whose index in <edges> should be returned.
  */
 mxCell.prototype.getEdgeIndex = function(edge)
 {
	 return mxUtils.indexOf(this.edges, edge);
 };
 
 /**
  * Function: getEdgeAt
  *
  * Returns the edge at the specified index in <edges>.
  * 
  * Parameters:
  * 
  * index - Integer that specifies the index of the edge to be returned.
  */
 mxCell.prototype.getEdgeAt = function(index)
 {
	 return (this.edges == null) ? null : this.edges[index];
 };
 
 /**
  * Function: insertEdge
  *
  * Inserts the specified edge into the edge array and returns the edge.
  * Will update the respective terminal reference of the edge.
  * 
  * Parameters:
  * 
  * edge - <mxCell> to be inserted into the edge array.
  * isOutgoing - Boolean that specifies if the edge is outgoing.
  */
 mxCell.prototype.insertEdge = function(edge, isOutgoing)
 {
	 if (edge != null)
	 {
		 edge.removeFromTerminal(isOutgoing);
		 edge.setTerminal(this, isOutgoing);
		 
		 if (this.edges == null ||
			 edge.getTerminal(!isOutgoing) != this ||
			 mxUtils.indexOf(this.edges, edge) < 0)
		 {
			 if (this.edges == null)
			 {
				 this.edges = [];
			 }
			 
			 this.edges.push(edge);
		 }
	 }
	 
	 return edge;
 };
 
 /**
  * Function: removeEdge
  *
  * Removes the specified edge from the edge array and returns the edge.
  * Will remove the respective terminal reference from the edge.
  * 
  * Parameters:
  * 
  * edge - <mxCell> to be removed from the edge array.
  * isOutgoing - Boolean that specifies if the edge is outgoing.
  */
 mxCell.prototype.removeEdge = function(edge, isOutgoing)
 {
	 if (edge != null)
	 {
		 if (edge.getTerminal(!isOutgoing) != this &&
			 this.edges != null)
		 {
			 var index = this.getEdgeIndex(edge);
			 
			 if (index >= 0)
			 {
				 this.edges.splice(index, 1);
			 }
		 }
		 
		 edge.setTerminal(null, isOutgoing);
	 }
	 
	 return edge;
 };
 
 /**
  * Function: removeFromTerminal
  *
  * Removes the edge from its source or target terminal.
  * 
  * Parameters:
  * 
  * isSource - Boolean that specifies if the edge should be removed from its
  * source or target terminal.
  */
 mxCell.prototype.removeFromTerminal = function(isSource)
 {
	 var terminal = this.getTerminal(isSource);
	 
	 if (terminal != null)
	 {
		 terminal.removeEdge(this, isSource);
	 }
 };
 
 /**
  * Function: hasAttribute
  * 
  * Returns true if the user object is an XML node that contains the given
  * attribute.
  * 
  * Parameters:
  * 
  * name - Name of the attribute.
  */
 mxCell.prototype.hasAttribute = function(name)
 {
	 var userObject = this.getValue();
	 
	 return (userObject != null &&
		 userObject.nodeType == mxConstants.NODETYPE_ELEMENT && userObject.hasAttribute) ?
		 userObject.hasAttribute(name) : userObject.getAttribute(name) != null;
 };
 
 /**
  * Function: getAttribute
  *
  * Returns the specified attribute from the user object if it is an XML
  * node.
  * 
  * Parameters:
  * 
  * name - Name of the attribute whose value should be returned.
  * defaultValue - Optional default value to use if the attribute has no
  * value.
  */
 mxCell.prototype.getAttribute = function(name, defaultValue)
 {
	 var userObject = this.getValue();
	 
	 var val = (userObject != null &&
		 userObject.nodeType == mxConstants.NODETYPE_ELEMENT) ?
		 userObject.getAttribute(name) : null;
		 
	 return (val != null) ? val : defaultValue;
 };
 
 /**
  * Function: setAttribute
  *
  * Sets the specified attribute on the user object if it is an XML node.
  * 
  * Parameters:
  * 
  * name - Name of the attribute whose value should be set.
  * value - New value of the attribute.
  */
 mxCell.prototype.setAttribute = function(name, value)
 {
	 var userObject = this.getValue();
	 
	 if (userObject != null &&
		 userObject.nodeType == mxConstants.NODETYPE_ELEMENT)
	 {
		 userObject.setAttribute(name, value);
	 }
 };
 
 /**
  * Function: clone
  *
  * Returns a clone of the cell. Uses <cloneValue> to clone
  * the user object. All fields in <mxTransient> are ignored
  * during the cloning.
  */
 mxCell.prototype.clone = function()
 {
	 var clone = mxUtils.clone(this, this.mxTransient);
	 clone.setValue(this.cloneValue());
	 
	 return clone;
 };
 
 /**
  * Function: cloneValue
  *
  * Returns a clone of the cell's user object.
  */
 mxCell.prototype.cloneValue = function()
 {
	 var value = this.getValue();
	 
	 if (value != null)
	 {
		 if (typeof(value.clone) == 'function')
		 {
			 value = value.clone();
		 }
		 else if (!isNaN(value.nodeType))
		 {
			 value = value.cloneNode(true);
		 }
	 }
	 
	 return value;
 };
 