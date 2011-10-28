(function() {
  window.game = (function($, JSON) {
    var bindEvents, drawCircle, drawCross, game, render, renderGrid, renderPlayers, renderSweep, updateScoreboard;
    updateScoreboard = function(gameState) {
      var name, player, _ref;
      $('#scoreboard').html('SCORES<br/><br/>');
      _ref = gameState.players;
      for (name in _ref) {
        player = _ref[name];
        $('<div>').html("" + name + ":  " + player.score).css('color', player.color).appendTo($('#scoreboard'));
      }
      return null;
    };
    render = function(gameState) {
      var canvas, context, renderInfo;
      canvas = document.getElementById('gamecanvas');
      if ((canvas != null) && (canvas.getContext != null)) {
        context = canvas.getContext('2d');
        if (context != null) {
          renderInfo = {
            canvasWidth: canvas.width,
            canvasHeight: canvas.height,
            cellWidth: canvas.width / gameState.grid.width,
            cellHeight: canvas.height / gameState.grid.height
          };
          context.clearRect(0, 0, canvas.width, canvas.height);
          renderGrid(context, gameState.grid, renderInfo);
          renderPlayers(context, gameState.players, renderInfo);
          return renderSweep(context, gameState.sweep, renderInfo);
        }
      }
    };
    renderSweep = function(context, sweep, renderInfo) {
      var height, startX, startY, width;
      startX = sweep.start.x * renderInfo.cellWidth;
      startY = sweep.start.y * renderInfo.cellHeight;
      width = (1 + sweep.end.x - sweep.start.x) * renderInfo.cellWidth;
      height = (1 + sweep.end.y - sweep.start.y) * renderInfo.cellHeight;
      context.fillStyle = 'rgba(128, 128, 128, 0.2)';
      return context.fillRect(startX, startY, width, height);
    };
    renderGrid = function(context, grid, renderInfo) {
      var x, y, _ref, _ref2;
      context.strokeStyle = '#000';
      for (x = 0, _ref = grid.width - 1; 0 <= _ref ? x <= _ref : x >= _ref; 0 <= _ref ? x++ : x--) {
        context.strokeRect(x * renderInfo.cellWidth, 0, renderInfo.cellWidth, renderInfo.canvasHeight);
      }
      for (y = 0, _ref2 = grid.height - 1; 0 <= _ref2 ? y <= _ref2 : y >= _ref2; 0 <= _ref2 ? y++ : y--) {
        context.strokeRect(0, y * renderInfo.cellHeight, renderInfo.canvasWidth, renderInfo.cellHeight);
      }
      return null;
    };
    renderPlayers = function(context, players, renderInfo) {
      var name, piece, pieceIndex, player, _ref;
      for (name in players) {
        player = players[name];
        context.fillStyle = player.color;
        context.fillRect(player.x * renderInfo.cellWidth, player.y * renderInfo.cellHeight, renderInfo.cellWidth, renderInfo.cellHeight);
        _ref = player.pieces;
        for (pieceIndex in _ref) {
          piece = _ref[pieceIndex];
          context.beginPath();
          drawCircle(context, piece.x, piece.y, renderInfo, 0.3);
          context.fill();
        }
        if (player.name === game.myName) {
          drawCross(context, player.x, player.y, renderInfo);
          context.stroke();
        }
      }
      return null;
    };
    drawCircle = function(context, x, y, renderInfo, radiusRatio) {
      var centerX, centerY;
      centerX = (x * renderInfo.cellWidth) + (renderInfo.cellWidth / 2);
      centerY = (y * renderInfo.cellHeight) + (renderInfo.cellHeight / 2);
      return context.arc(centerX, centerY, Math.min(renderInfo.cellWidth, renderInfo.cellHeight) * radiusRatio, 0, Math.PI * 2, true);
    };
    drawCross = function(context, x, y, renderInfo) {
      var centerX, centerY;
      centerX = (x * renderInfo.cellWidth) + (renderInfo.cellWidth / 2);
      centerY = (y * renderInfo.cellHeight) + (renderInfo.cellHeight / 2);
      context.strokeStyle = '#000';
      context.beginPath();
      context.moveTo(centerX - (renderInfo.cellWidth / 3), centerY);
      context.lineTo(centerX + (renderInfo.cellWidth / 3), centerY);
      context.moveTo(centerX, centerY - (renderInfo.cellHeight / 3));
      return context.lineTo(centerX, centerY + (renderInfo.cellHeight / 3));
    };
    bindEvents = function(connection, gameState) {
      var events;
      events = connection.game;
      events.movePlayer = function(name, position) {
        var player;
        player = gameState.players[name];
        player.x = position.x;
        player.y = position.y;
        return render(gameState);
      };
      events.startGame = function(dimensions) {
        gameState.grid = dimensions;
        $('#controls').show();
        return render(gameState);
      };
      events.addPlayer = function(name, color, position) {
        gameState.players[name] = {
          name: name,
          color: color,
          x: position.x,
          y: position.y,
          pieces: [],
          score: 0
        };
        updateScoreboard(gameState);
        return render(gameState);
      };
      events.addPiece = function(name, position) {
        gameState.players[name].pieces.push({
          x: position.x,
          y: position.y
        });
        return render(gameState);
      };
      events.movePiece = function(name, origin, destination) {
        var match;
        match = $.grep(gameState.players[name].pieces, function(piece, index) {
          return piece.x === origin.x && piece.y === origin.y;
        });
        match[0].x = destination.x;
        match[0].y = destination.y;
        return render(gameState);
      };
      events.updateSweep = function(start, end) {
        gameState.sweep = {
          start: start,
          end: end
        };
        return render(gameState);
      };
      events.removePiece = function(name, position) {
        var player;
        player = gameState.players[name];
        player.pieces = $.grep(player.pieces, function(piece, index) {
          return !(piece.x === position.x && piece.y === position.y);
        });
        return render(gameState);
      };
      events.updateScores = function(scores) {
        var index, score;
        for (index in scores) {
          score = scores[index];
          gameState.players[score.name].score = score.value;
        }
        return updateScoreboard(gameState);
      };
      return events.removePlayer = function(name) {
        delete gameState.players[name];
        $('#messages').prepend("<div>" + name + " left.</div>");
        updateScoreboard(gameState);
        return render(gameState);
      };
    };
    return game = {
      createNewGameState: function() {
        return {
          grid: {},
          players: {},
          sweep: {
            start: {
              x: 0,
              y: 0
            },
            end: {
              x: 0,
              y: 9
            }
          }
        };
      },
      bindEvents: bindEvents
    };
  })(jQuery, JSON);
}).call(this);
