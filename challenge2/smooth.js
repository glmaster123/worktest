const KalmanFilter = require('kalmanjs');
const fs = require('fs');

const rawData = fs.readFileSync('../region.json');
const region = JSON.parse(rawData);
const kalmanFilterX = new KalmanFilter({ R: 0.01, Q: 3 });
const kalmanFilterY = new KalmanFilter({ R: 0.01, Q: 3 });
const smoothed = { coordinates: [] };

smoothed.coordinates = region.coordinates.map((coordinate) => {
    return [
        kalmanFilterX.filter(coordinate[0]),
        kalmanFilterY.filter(coordinate[1]),
    ];
});

fs.writeFileSync('smoothed.json', JSON.stringify(smoothed));

let kmlString = '';

smoothed.coordinates.forEach((coordinate) => {
    kmlString += `${coordinate[1]},${coordinate[0]},0 `;
});

fs.writeFileSync('kml.txt', kmlString);