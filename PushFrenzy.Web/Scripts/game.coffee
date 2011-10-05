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
	
	updateHandlers =
		NewGame: (gameState, stateUpdate) ->
			gameState.grid = stateUpdate.dimensions
			$('#controls').show()        
		PlayerAdded: (gameState, stateUpdate) ->
			gameState.players[stateUpdate.name] = stateUpdate
			gameState.players[stateUpdate.name].pieces = []
			gameState.players[stateUpdate.name].score = 0
			updateScoreboard(gameState)        
		PlayerMoved: (gameState, stateUpdate) ->
			player = gameState.players[stateUpdate.player]
			player.x = stateUpdate.x
			player.y = stateUpdate.y        
		PieceAdded: (gameState, stateUpdate) ->
			gameState.players[stateUpdate.owner].pieces.push(stateUpdate)        
		PieceMoved: (gameState, stateUpdate) ->
			match = $.grep(
				gameState.players[stateUpdate.owner].pieces, 
				(piece, index) -> piece.x is stateUpdate.origin.x and piece.y is stateUpdate.origin.y            
			)
			match[0].x = stateUpdate.destination.x
			match[0].y = stateUpdate.destination.y        
		PieceRemoved: (gameState, stateUpdate) ->
			player = gameState.players[stateUpdate.owner]
			player.pieces = $.grep(
				player.pieces, 
				(piece, index) -> not (piece.x is stateUpdate.x and piece.y is stateUpdate.y)                   
			)
		SweepUpdated: (gameState, stateUpdate) ->
			gameState.sweep = stateUpdate        
		ScoresUpdated: (gameState, stateUpdate) ->
			for index, score of stateUpdate.scores
				gameState.players[score.name].score = score.value				
			updateScoreboard(gameState)        
		PlayerRemoved: (gameState, stateUpdate) ->
			delete gameState.players[stateUpdate.name]
			$('#messages').prepend("<div>#{stateUpdate.name} left.</div>")
			updateScoreboard(gameState)        
	
	game = 
		createNewGameState: -> {
			grid: {}
			players: {}
			sweep:
				start: { x: 0, y: 0 }
				end: { x: 0, y: 9 }				
		}
		processMessage: (gameState, stringPayload) ->
			stateUpdates = JSON.parse(stringPayload)
			for index, updateItem of stateUpdates
				updateHandlers[updateItem.type](gameState, updateItem.body)
			render(gameState)    

)(jQuery, JSON)