function NgGridWrapper() {
	this.appName = 'AeyeNgApp';
	this.ctrlName = 'NgCtrl';
	this.gridName = 'NgGrid';
	this.nameIndex = 0;
	this.providers = {};	
};

NgGridWrapper.prototype.getCtrlId = function() {
	return this.ctrlName + this.nameIndex;
}

NgGridWrapper.prototype.getGridId = function() {
	return this.gridName + this.nameIndex;
}

NgGridWrapper.prototype.generateNextNameIndex = function() {
	this.nameIndex++;
}

NgGridWrapper.prototype.initApp = function() {
	angular.module(this.appName, ['ngGrid'], function($controllerProvider, $compileProvider, $provide) {
		  providers = {
		      $controllerProvider: $controllerProvider,
		      $compileProvider: $compileProvider,
		      $provide: $provide
		  };
	});

	angular.bootstrap($('body'), [this.appName]);
	console.log('Ng app initialised');
}

NgGridWrapper.prototype.registerController = function(options) {
	var queueLen = angular.module(this.appName)._invokeQueue.length;
	this.generateNextNameIndex();

	angular.module(this.appName).controller(this.getCtrlId(), function($scope, $rootScope) {
		$scope.gridData = [];
		$scope.gridColumnDefs = [];
		$scope.gridAfterSelectionChange = null;

		$scope.gridOptions = {
				data: 'gridData',
				columnDefs: 'gridColumnDefs',
				enableCellSelection: false,
				enableRowSelection: true,
				enableCellEdit: false,
				showFilter: false,
				multiSelect: options['multiSelect'] === true,
				showSelectionCheckbox: true,
				afterSelectionChange: function (rowItem) {
					if ($scope.gridAfterSelectionChange != null) {
							$scope.gridAfterSelectionChange($scope.gridOptions.$gridScope.selectedItems);
					}
				}
		};

		$scope.setGridColumnDefs = function(columnDefs) {
		    $scope.gridColumnDefs = columnDefs;
	  };

		$scope.setGridData = function(data) {
	    $scope.gridData = data;
	  };

		$scope.appendGridData = function(data) {
			for (var i = 0, n = data.length; i < n; i++)
	    	$scope.gridData.push(data[i]);
	  };

		$scope.setGridAfterSelectionChange = function(eventHandler) {
	    	$scope.gridAfterSelectionChange = eventHandler;
		};
	});

	var queue = angular.module(this.appName)._invokeQueue;

	for(var i = queueLen, n = queue.length; i < n; i++) {
		  var call = queue[i];
		  var provider = providers[call[0]];

		  if (provider)
		      provider[call[1]].apply(provider, call[2]);
	}
}

NgGridWrapper.prototype.createGrid = function() {
	var ctrlId = this.getCtrlId();
	var gridId = this.getGridId();
	var el = $('<div id="' + ctrlId + '" ng-controller="' + ctrlId + '"><div id="' + gridId + '" class="gridStyle" ng-grid="gridOptions"/></div>')[0];
	console.log('Created DOM elements for "' + ctrlId + '" controller and "' + gridId + '" grid: ' + el);
	return el;
}

NgGridWrapper.prototype.compileGrid = function(gridCtrlId) {
	console.log('Compiling DOM of "' + gridCtrlId + '" controller');

	$('body').injector().invoke(function($compile, $rootScope) {
		  $compile($('#' + gridCtrlId))($rootScope);
		  $rootScope.$apply();
	});

	console.log('Compiled DOM of "' + gridCtrlId + '" controller');
}

NgGridWrapper.prototype.setGridEventHandler = function(gridId, eventName, eventHandler) {
	var scope = angular.element($("#" + gridId)).scope();
	var eventNameLc = eventName.toLowerCase();

  scope.$apply(function() {
		switch (eventNameLc) {
			case 'afterselectionchange':
				scope.setGridAfterSelectionChange(eventHandler);
				break;
		}
	});

	console.log('Set \'' + eventName + '\' DOM event handler for "' + gridId + '" grid');
}

NgGridWrapper.prototype.setGridLayout = function(gridId, layoutArray) {
	var e = document.getElementById(gridId);
	var style = e.style;
	style.position = layoutArray['position'];
	style.top = layoutArray['top'];
	style.left = layoutArray['left'];
	style.width = layoutArray['width'];
	style.height = layoutArray['height'];

	console.log('Set layout for "' + gridId + '" grid');
}

NgGridWrapper.prototype.setGridColumns = function(gridId, gridColumnDefs) {
  var scope = angular.element($("#" + gridId)).scope();

  scope.$apply(function() {
			scope.setGridColumnDefs(gridColumnDefs);
  });

	console.log('Set columns for "' + gridId + '" grid');
}

NgGridWrapper.prototype.setGridData = function(gridId, gridData) {
	var scope = angular.element($("#" + gridId)).scope();

  scope.$apply(function() {
			scope.appendGridData(gridData);
  });

	console.log('Set data for "' + gridId + '" grid');
}
