var game = (function ($, JSON) {

    var g = {};

    g.createNewGameState = function () {
        return {
            grid: {},
            players: {},
            sweep: {
                start: { x: 0, y: 0 },
                end: { x: 0, y: 9 }
            }
        };
    };

    var updateHandlers = {
        NewGame: function (gameState, stateUpdate) {
            gameState.grid = stateUpdate.dimensions;
            $('#controls').show();
        },
        PlayerAdded: function (gameState, stateUpdate) {
            gameState.players[stateUpdate.name] = stateUpdate;
            gameState.players[stateUpdate.name].pieces = [];
            gameState.players[stateUpdate.name].score = 0;
            updateScoreboard(gameState);
        },
        PlayerMoved: function (gameState, stateUpdate) {
            var player = gameState.players[stateUpdate.player];
            player.x = stateUpdate.x;
            player.y = stateUpdate.y;
        },
        PieceAdded: function (gameState, stateUpdate) {
            gameState.players[stateUpdate.owner].pieces.push(stateUpdate);
        },
        PieceMoved: function (gameState, stateUpdate) {
            var match = $.grep(gameState.players[stateUpdate.owner].pieces, function (piece, index) {
                return piece.x === stateUpdate.origin.x && piece.y === stateUpdate.origin.y;
            });

            match[0].x = stateUpdate.destination.x;
            match[0].y = stateUpdate.destination.y;
        },
        PieceRemoved: function (gameState, stateUpdate) {
            var player = gameState.players[stateUpdate.owner];
            player.pieces = $.grep(player.pieces, function (piece, index) {
                return piece.x !== stateUpdate.x || piece.y !== stateUpdate.y;
            });
        },
        SweepUpdated: function (gameState, stateUpdate) {
            gameState.sweep = stateUpdate;
        },
        ScoresUpdated: function (gameState, stateUpdate) {
            $.each(stateUpdate.scores, function (index, score) {
                gameState.players[score.name].score = score.value;
            });
            updateScoreboard(gameState);
        },
        PlayerRemoved: function (gameState, stateUpdate) {
            delete gameState.players[stateUpdate.name];
            $('#messages').prepend('<div>' + stateUpdate.name + ' left.</div>');
            updateScoreboard(gameState);
        },
    }

    g.processMessage = function (gameState, stringPayload) {
        var stateUpdates = JSON.parse(stringPayload);
        $.each(stateUpdates, function (index, updateItem) {
            updateHandlers[updateItem.type](gameState, updateItem.body);
        });

        render(gameState);
        //$('#messages').prepend('<div>' + JSON.stringify(gameState) + '</div>');
        //$('#messages').prepend('<div>' + JSON.stringify(stringPayload) + '</div>');
    };

    function updateScoreboard(gameState) {
        $('#scoreboard').html('SCORES<br/><br/>');
        $.each(gameState.players, function (name, player) {
            var div = $('<div>');
            div.html(name + ':  ' + player.score);
            div.css('color', player.color);
            $('#scoreboard').append(div);
        });
    }

    function render(gameState) {
        var canvas = document.getElementById('gamecanvas');
        if (canvas && canvas.getContext) {
            var context = canvas.getContext('2d');
            if (context) {

                var renderInfo = {
                    canvasWidth: canvas.width,
                    canvasHeight: canvas.height,
                    cellWidth: canvas.width / gameState.grid.width,
                    cellHeight: canvas.height / gameState.grid.height
                }

                context.clearRect(0, 0, canvas.width, canvas.height);
                renderGrid(context, gameState.grid, renderInfo);
                renderPlayers(context, gameState.players, renderInfo);
                renderSweep(context, gameState.sweep, renderInfo);
            }
        }
    }

    function renderSweep(context, sweep, renderInfo) {
        var startX, startY, width, height;

        startX = sweep.start.x * renderInfo.cellWidth;
        startY = sweep.start.y * renderInfo.cellHeight;
        width = (1 + sweep.end.x - sweep.start.x) * renderInfo.cellWidth;
        height = (1 + sweep.end.y - sweep.start.y) * renderInfo.cellHeight;

        context.fillStyle = 'rgba(128, 128, 128, 0.2)';
        context.fillRect(startX, startY, width, height);
    }

    function renderGrid(context, grid, renderInfo) {
        context.strokeStyle = '#000';

        for (var x = 0; x < grid.width; x++) {
            context.strokeRect(x * renderInfo.cellWidth, 0, renderInfo.cellWidth, renderInfo.canvasHeight);
        }

        for (var y = 0; y < grid.height; y++) {
            context.strokeRect(0, y * renderInfo.cellHeight, renderInfo.canvasWidth, renderInfo.cellHeight);
        }

    }

    function renderPlayers(context, players, renderInfo) {
        $.each(players, function (name, player) {
            context.fillStyle = player.color;
            context.fillRect(player.x * renderInfo.cellWidth, player.y * renderInfo.cellHeight, renderInfo.cellWidth, renderInfo.cellHeight);

            $.each(player.pieces, function (pieceIndex, piece) {
                context.beginPath();
                drawCircle(context, piece.x, piece.y, renderInfo, 0.3);
                context.fill();
            });

            if (player.name == g.myName) {
                drawCross(context, player.x, player.y, renderInfo);
                context.stroke();
            }

        });
    }

    function drawCircle(context, x, y, renderInfo, radiusRatio) {
        var centerX = (x * renderInfo.cellWidth) + (renderInfo.cellWidth / 2);
        var centerY = (y * renderInfo.cellHeight) + (renderInfo.cellHeight / 2);
        context.arc(centerX, centerY, Math.min(renderInfo.cellWidth, renderInfo.cellHeight) * radiusRatio, 0, Math.PI * 2, true);
    }

    function drawCross(context, x, y, renderInfo) {
        var centerX = (x * renderInfo.cellWidth) + (renderInfo.cellWidth / 2);
        var centerY = (y * renderInfo.cellHeight) + (renderInfo.cellHeight / 2);

        context.strokeStyle = '#000';
        context.beginPath();
        context.moveTo(centerX - (renderInfo.cellWidth / 3), centerY);
        context.lineTo(centerX + (renderInfo.cellWidth / 3), centerY);
        context.moveTo(centerX, centerY - (renderInfo.cellHeight / 3));
        context.lineTo(centerX, centerY + (renderInfo.cellHeight / 3));
    }

    return g;

}(jQuery, JSON));