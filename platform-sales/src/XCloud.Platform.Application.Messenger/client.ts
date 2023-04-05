const build_client = () => {
    const client = new WebSocket("wss://localhost:8080");

    client.onclose = e => {
        console.log(e);
    };

    client.onopen = e => {
        console.log(e);
    };

    client.onerror = e => {
        console.log(e);
    };

    client.onmessage = e => {
        console.log(e);
    };

    return client;
};