'use strict';

var itemTiler = itemTiler || {};

(function (window, document) {
    var minWidth = 200;
    var margin = 17;
    var cols = [];

    var getShortestColumn = function () {
        var min = Math.min.apply(Math, cols);
        for (var i = 0; i < cols.length; i++) {
            if (cols[i] === min) {
                return i;
            }
        }
        return 0;
    }

    var layout = function () {
        var containers = document.getElementsByClassName('tileable-items');
        for (var c = 0; c < containers.length; c++) {
            var container = containers[c];
            var minItemWidth = minWidth;
            if (container.dataset.tileWidth != undefined) {
                minItemWidth = parseInt(container.dataset.tileWidth);
                if (isNaN(minItemWidth)) {
                    minItemWidth = minWidth;
                }
            }
            var width = container.offsetWidth;
            var nbrCols = Math.max(1, Math.floor((width + margin) / (minItemWidth + margin)));
            cols = Array.apply(null, Array(nbrCols)).map(function () { return 0; });
            var colWidth = Math.floor((width - (nbrCols - 1) * margin) / nbrCols);
            for (var i = 0; i < container.children.length; i++) {
                var child = container.children[i];
                var col = getShortestColumn();
                child.style.width = colWidth + 'px';
                child.style.position = 'absolute';
                child.style.marginLeft = (col * (margin + colWidth)) + 'px';
                if (cols[col] !== 0) {
                    cols[col] += 13;
                }
                child.style.marginTop = cols[col] + 'px';
                cols[col] += child.offsetHeight + margin;
            }
            var max = Math.max.apply(Math, cols);
            container.style.height = max + 'px';
        }
    }

    window.addEventListener('resize', function () {
        layout();
    }, true);

    window.addEventListener('load', function () {
        layout();
    }, true);

    layout();

    // make public
    itemTiler.layout = layout;

})(window, document);