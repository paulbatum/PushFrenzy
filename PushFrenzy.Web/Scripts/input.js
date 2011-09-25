game.input = (function ($, game) {

    var input = {};

    input.prepareNameBox = function () {
        //$('#namebox').val('Player_' + Math.floor(Math.random()*1000));

        updatePlayEnabled();
        $('#namebox')
                .keyup(updatePlayEnabled)
                .focus();
    };

    input.bindControls = function (websocket) {
        bindTouchControls(websocket);
        bindKeyboardControls(websocket);
    }



    function updatePlayEnabled() {
        if ($('#namebox').val().length === 0)
            $('#playbutton').attr('disabled', 'disabled');
        else
            $('#playbutton').removeAttr('disabled');
        };

    function bindKeyboardControls(websocket) {
        var moveKeyMap = {
            Down: ['down', 's'],
            Up: ['up', 'w'],
            Left: ['left', 'a'],
            Right: ['right', 'd']
        };

        $.each(moveKeyMap, function (direction, keys) {
            $.each(keys, function (_, key) {
                $(document).bind('keydown', key, function () { move(websocket, direction); });
            });
        });
    };

    function bindTouchControls(websocket) {
        var interval;

        $('#arrorImg').bind('dragstart', function (event) {
            event.preventDefault();
        });

        $('#arrowMap area').click(function (event) {
            event.preventDefault();
        }).mousedown(function (event) {
            var elem = this;
            var moveFn = function () {
                move(websocket, $(elem).attr('alt'));
            };
            moveFn();
            if (interval) {
                clearInterval(interval);
            }
            interval = setInterval(moveFn, 200);
            event.preventDefault();
        }).bind('mouseup mouseleave', function () {
            clearInterval(interval);
        });
    };

    function move (websocket, direction) {
        var msg = {
            Type: 'PlayerMoveCommand',
            Direction: direction
        };

        websocket.send(JSON.stringify(msg));
    }

    return input;
}(jQuery, game));