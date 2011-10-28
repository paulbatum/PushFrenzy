window.game = (($, JSON) ->

	updateScoreboard = (gameState) ->
		$('#scoreboard').html('SCORES<br/><br/>')
		for name, player of gameState.players
			$('<div>')
				.html("#{name}:  #{player.score}")
				.css('color', player.color)
				.appendTo($('#scoreboard'))
		null

	render = (gameState) ->
		canvas = document.getElementById('gamecanvas')
		if canvas? and canvas.getContext?
			context = canvas.getContext('2d')
			if context?
				renderInfo =
					canvasWidth: canvas.width
					canvasHeight: canvas.height
					cellWidth: canvas.width / gameState.grid.width
					cellHeight: canvas.height / gameState.grid.height                

				context.clearRect(0, 0, canvas.width, canvas.height)
				renderGrid(context, gameState.grid, renderInfo)
				renderPlayers(context, gameState.players, renderInfo)
				renderSweep(context, gameState.sweep, renderInfo)		
	
	renderSweep = (context, sweep, renderInfo) ->
		startX = sweep.start.x * renderInfo.cellWidth
		startY = sweep.start.y * renderInfo.cellHeight
		width = (1 + sweep.end.x - sweep.start.x) * renderInfo.cellWidth
		height = (1 + sweep.end.y - sweep.start.y) * renderInfo.cellHeight

		context.fillStyle = 'rgba(128, 128, 128, 0.2)'
		context.fillRect(startX, startY, width, height)
	
	renderGrid = (context, grid, renderInfo) ->
		context.strokeStyle = '#000'
		for x in [0..grid.width-1]
			context.strokeRect(x * renderInfo.cellWidth, 0, renderInfo.cellWidth, renderInfo.canvasHeight)
		for y in [0..grid.height-1]
			context.strokeRect(0, y * renderInfo.cellHeight, renderInfo.canvasWidth, renderInfo.cellHeight)
		null

	renderPlayers = (context, players, renderInfo) ->
		for name, player of players
			context.fillStyle = player.color
			context.fillRect(player.x * renderInfo.cellWidth, player.y * renderInfo.cellHeight, renderInfo.cellWidth, renderInfo.cellHeight)

			for pieceIndex, piece of player.pieces
				context.beginPath()
				drawCircle(context, piece.x, piece.y, renderInfo, 0.3)
				context.fill()

			if player.name is game.myName
				drawCross(context, player.x, player.y, renderInfo)
				context.stroke()
		null

	drawCircle = (context, x, y, renderInfo, radiusRatio) ->
		centerX = (x * renderInfo.cellWidth) + (renderInfo.cellWidth / 2)
		centerY = (y * renderInfo.cellHeight) + (renderInfo.cellHeight / 2)
		context.arc(centerX, centerY, Math.min(renderInfo.cellWidth, renderInfo.cellHeight) * radiusRatio, 0, Math.PI * 2, true)
	
	drawCross = (context, x, y, renderInfo) ->
		centerX = (x * renderInfo.cellWidth) + (renderInfo.cellWidth / 2)
		centerY = (y * renderInfo.cellHeight) + (renderInfo.cellHeight / 2)
		context.strokeStyle = '#000'
		context.beginPath()
		context.moveTo(centerX - (renderInfo.cellWidth / 3), centerY)
		context.lineTo(centerX + (renderInfo.cellWidth / 3), centerY)
		context.moveTo(centerX, centerY - (renderInfo.cellHeight / 3))
		context.lineTo(centerX, centerY + (renderInfo.cellHeight / 3))
	
	bindEvents = (connection, gameState) ->
		events = connection.game;
	
		events.movePlayer = (name, position) ->
			player = gameState.players[name]
			player.x = position.x
			player.y = position.y
			render(gameState)
		events.startGame = (dimensions) ->
			gameState.grid = dimensions
			$('#controls').show()        
			render(gameState)
		events.addPlayer = (name, color, position) ->
			gameState.players[name] =
				name: name
				color: color
				x: position.x
				y: position.y
				pieces: []
				score: 0
			updateScoreboard(gameState)        
			render(gameState)
		events.addPiece = (name, position) ->
			gameState.players[name].pieces.push({
				x: position.x
				y: position.y
			})
			render(gameState)
		events.movePiece = (name, origin, destination) ->
			match = $.grep(
				gameState.players[name].pieces, 
				(piece, index) -> piece.x is origin.x and piece.y is origin.y            
			)
			match[0].x = destination.x
			match[0].y = destination.y        
			render(gameState)
		events.updateSweep = (start, end) ->
			gameState.sweep =
				start:start
				end: end	
			render(gameState)
		events.removePiece = (name, position) ->
			player = gameState.players[name]
			player.pieces = $.grep(
				player.pieces, 
				(piece, index) -> not (piece.x is position.x and piece.y is position.y)                   
			)
			render(gameState)
		events.updateScores = (scores) ->		
			for index, score of scores
				gameState.players[score.name].score = score.value					
			updateScoreboard(gameState)        
		events.removePlayer = (name) ->
			delete gameState.players[name]
			$('#messages').prepend("<div>#{name} left.</div>")
			updateScoreboard(gameState)        
			render(gameState)
	
	game = 
		createNewGameState: -> {
			grid: {}
			players: {}
			sweep:
				start: { x: 0, y: 0 }
				end: { x: 0, y: 9 }				
		}
		bindEvents: bindEvents 

)(jQuery, JSON)