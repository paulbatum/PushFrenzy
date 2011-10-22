game.input = (($, game) ->
	updatePlayEnabled = ->
		if($('#namebox').val().length == 0)
			$('#playbutton').attr('disabled', 'disabled')
		else
			$('#playbutton').removeAttr('disabled')
	
	bindKeyboardControls = (connection) ->
		moveKeyMap =
			Down: ['down', 's']
			Up: ['up', 'w']
			Left: ['left', 'a']
			Right: ['right', 'd']        
		for direction, keys of moveKeyMap
			for key in keys			
				do (direction, key) -> $(document).bind('keydown', key, -> move(connection, direction)) 									
		null		
	
	bindTouchControls = (connection) ->
		interval = {}
		$('#arrorImg').bind('dragstart', (event) -> event.preventDefault())
		$('#arrowMap area')
			.click( (event) -> event.preventDefault())
			.mousedown( (event) ->
				moveFn = -> move(connection, $(event.target).attr('alt'))
				moveFn()
				clearInterval(interval) if interval
				interval = setInterval(moveFn, 200)
				event.preventDefault())
			.bind('mouseup mouseleave', -> clearInterval(interval))
	
	move = (connection, direction) ->				
		connection.game.move(direction)
	
	input = 
		prepareNameBox: -> 
			updatePlayEnabled()
			$('#namebox').keyup(updatePlayEnabled).focus()
		bindControls: (connection) ->
			bindTouchControls(connection)
			bindKeyboardControls(connection)
	
)(jQuery, game)