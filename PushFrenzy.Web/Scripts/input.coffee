game.input = (($, game) ->
	updatePlayEnabled = ->
		if($('#namebox').val().length == 0)
			$('#playbutton').attr('disabled', 'disabled')
		else
			$('#playbutton').removeAttr('disabled')
	
	bindKeyboardControls = (websocket) ->
		moveKeyMap =
			Down: ['down', 's']
			Up: ['up', 'w']
			Left: ['left', 'a']
			Right: ['right', 'd']        
		for direction, keys of moveKeyMap
			for key in keys			
				do (direction, key) -> $(document).bind('keydown', key, -> move(websocket, direction)) 									
		null		
	
	bindTouchControls = (websocket) ->
		interval = {}
		$('#arrorImg').bind('dragstart', (event) -> event.preventDefault())
		$('#arrowMap area')
			.click( (event) -> event.preventDefault())
			.mousedown( (event) ->
				moveFn = -> move(websocket, $(event.target).attr('alt'))
				moveFn()
				clearInterval(interval) if interval
				interval = setInterval(moveFn, 200)
				event.preventDefault())
			.bind('mouseup mouseleave', -> clearInterval(interval))
	
	move = (websocket, direction) ->
		msg =
			Type: 'PlayerMoveCommand'
			Direction: direction
		
		websocket.send(JSON.stringify(msg))
	
	input = 
		prepareNameBox: -> 
			updatePlayEnabled()
			$('#namebox').keyup(updatePlayEnabled).focus()
		bindControls: (websocket) ->
			bindTouchControls(websocket)
			bindKeyboardControls(websocket)
	
)(jQuery, game)