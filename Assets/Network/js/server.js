const WebSocket = require('ws');
const server = new WebSocket.Server({ port: 8080 });

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', (message) => {
        console.log('Received:', message);
        const data = JSON.parse(message);

        // Generate new x and y coordinates for the object with the given unique id
        const updatedData = {
            id: data.id,
            x: data.x + Math.random() - 0.5,
            y: data.y + Math.random() - 0.5,
        };

        // Broadcast the updated coordinates to all connected clients
        server.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(JSON.stringify(updatedData));
            }
        });
    });
});

console.log('WebSocket server is running on port 8080');
