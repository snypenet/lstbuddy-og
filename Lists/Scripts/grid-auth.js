Array.prototype.contains = function(i){
    var found = false;
    for (var index in this) {
        if(this[index] === i){
            found = true;
            break;
        }
    }
    return found;
};

var authGrid = {
	selectedCells: [],
	domContainer: "",
    multiSelectEnabled: false,
	createGrid: function (i) {
	    if (!i || !i.gridContainer) {
	        console.error("Object is null or container is null");
	        return;
	    }

	    this.selectedCells = [];

	    this.domContainer = i.gridContainer;
	    if (i.width) {
	        this.width = i.width;
	    }

	    if (i.height) {
            this.height = i.height
	    }

	    if (i.colors) {
	        this.levelsOfColor = i.colors;
	    }

	    if (i.defaultColor) {
	        this.defaultColor = i.defaultColor;
	    }

	    if (i.patternContainer) {
	        this.inputContainer = i.patternContainer;
	    }

	    if (i.multiEnabled) {
	        this.multiSelectEnabled = new Boolean(i.multiEnabled);
	    }

		this.domContainer.html(this.gridHTML);
		this.domContainer.find(".gridAuthContainer table tr td").unbind("click").bind("click", function(){
			var row = $(this).closest("tr").attr("data-row");
			var column = $(this).attr("data-column");

			if ($(this).attr("data-select-count")) {
			    $(this).attr("data-select-count", Number($(this).attr("data-select-count")) + 1);
			} else {
			    $(this).attr("data-select-count", "1");
			}

            //if a level of color exists then set it
			if (authGrid.levelsOfColor[Number($(this).attr("data-select-count")) - 1]) {
			    $(this).css("background-color", authGrid.levelsOfColor[Number($(this).attr("data-select-count")) - 1]);
			} else {
			    $(this).css("background-color", authGrid.defaultColor);
			}

			$(this).attr("data-selected", "true").addClass("gridCellSelected");
			authGrid.addToPattern(row + column);
		});
	},
    inputContainer:"",
	height: 4,
	width: 4,
    defaultColor:"#4e4e4e",
    levelsOfColor: [],
	addToPattern: function (cell) {
	    if (this.multiSelectEnabled || !this.selectedCells.contains(cell)) {
	        this.selectedCells.push(cell);
	        if (this.inputContainer && this.inputContainer.length > 0) {
	            this.inputContainer.val(this.pattern());
	        }
	    }
	},
	reset:function(){
	    this.selectedCells = [];
	    if (this.domContainer) {
	        this.domContainer.find(".gridAuthContainer table tr td")
                             .removeClass("gridCellSelected")
                             .removeAttr("data-select-count")
                             .css("background-color", "");
	    }

		if (this.inputContainer && this.inputContainer.length > 0) {
		    this.inputContainer.val("");
		}

		this.domContainer.trigger("gridreset");
	},
	setPattern:function(pattern){
	    if (!pattern || pattern.length == 0) {
	        return;
	    }

	    pattern = pattern.split(":");

	    for (var combo in pattern) {
	        var $gridCell = $("[data-row=" + pattern[combo][0] + "]").find("[data-column=" + pattern[combo][1] + "]");
	        $gridCell.attr("data-selected", "true");
	        if ($gridCell.attr("data-select-count")) {
	            $gridCell.attr("data-select-count", Number($gridCell.attr("data-select-count")) + 1);
	        } else {
	            $gridCell.attr("data-select-count", 1);
	        }

	        //if a level of color exists then set it
	        if (authGrid.levelsOfColor[Number($gridCell.attr("data-select-count")) - 1]) {
	            $gridCell.css("background-color", authGrid.levelsOfColor[Number($gridCell.attr("data-select-count")) - 1]);
	        } else {
	            $gridCell.css("background-color", authGrid.defaultColor);
	        }
	    }
	},
	pattern:function(){
		return this.selectedCells.join(":");
	},
	gridHTML: function () {
	    var $gridContainer = $("<div>").addClass("gridAuthContainer").append($("<table>"));

        //make sure number is positive
	    authGrid.height = Math.abs(Number(authGrid.height));

        //reset height to 26 if it is greater than 26
	    if (Number(authGrid.height) > 52) {
	        authGrid.height = 52;
	    }

        //make sure value is positvie
	    authGrid.width = Math.abs(Number(authGrid.width));

	    for (var i = 0; i < Number(authGrid.height) ; i++) {
	        $gridContainer.find("table").append($("<tr>").attr("data-row", keys[i]));

	        for (var j = 0; j < Number(authGrid.width) ; j++){
	            $gridContainer.find("tr[data-row=" + keys[i] + "]").append($("<td>").attr("data-column", j + 1));
            }
	    }

	    return $gridContainer;
	}
};

var keys = {
    "0":"A",
    "1":"B",
    "2":"C",
    "3":"D",
    "4":"E",
    "5":"F",
    "6":"G",
    "7":"H",
    "8":"I",
    "9":"J",
    "10":"K",
    "11":"L",
    "12":"M",
    "13":"N",
    "14":"O",
    "15":"P",
    "16":"Q",
    "17":"R",
    "18":"S",
    "19":"T",
    "20":"U",
    "21":"V",
    "22":"W",
    "23":"X",
    "24":"Y",
    "25":"Z",
    "26":"a",
    "27":"b",
    "28":"c",
    "29":"d",
    "30":"e",
    "31":"f",
    "32":"g",
    "33":"h",
    "34":"i",
    "35":"j",
    "36":"k",
    "37":"l",
    "38":"m",
    "39":"n",
    "40":"o",
    "41":"p",
    "42":"q",
    "43":"r",
    "44":"s",
    "45":"t",
    "46":"u",
    "47":"v",
    "48":"w",
    "49":"x",
    "50":"y",
    "51":"z",
};

