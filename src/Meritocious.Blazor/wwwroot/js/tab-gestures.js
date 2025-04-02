// Gesture tracking state
let swipeState = {
    startX: 0,
    startTime: 0,
    isTracking: false
};

const SWIPE_THRESHOLD = 100;
const SWIPE_VELOCITY_THRESHOLD = 0.5;

window.initTabGestures = function(container) {
    // Prevent default gestures
    container.addEventListener('gesturestart', (e) => e.preventDefault());
    container.addEventListener('gesturechange', (e) => e.preventDefault());
    container.addEventListener('gestureend', (e) => e.preventDefault());

    if ('scrollBehavior' in document.documentElement.style) {
        container.style.scrollBehavior = 'smooth';
    }
};

window.startSwipeTracking = function(clientX) {
    swipeState = {
        startX: clientX,
        startTime: Date.now(),
        isTracking: true
    };
};

window.updateSwipeTracking = function(clientX) {
    if (!swipeState.isTracking) return { isValid: false };

    const distance = clientX - swipeState.startX;
    const direction = distance > 0 ? 'right' : 'left';
    const isValid = Math.abs(distance) > SWIPE_THRESHOLD / 2;

    return {
        isValid,
        direction,
        distance: Math.abs(distance)
    };
};

window.endSwipeTracking = function() {
    if (!swipeState.isTracking) return { isValid: false };

    const distance = clientX - swipeState.startX;
    const endTime = Date.now();
    const duration = endTime - swipeState.startTime;
    const velocity = Math.abs(distance) / duration;
    const direction = distance > 0 ? 'right' : 'left';

    const isValid = Math.abs(distance) > SWIPE_THRESHOLD && 
                    velocity > SWIPE_VELOCITY_THRESHOLD;

    swipeState.isTracking = false;

    return {
        isValid,
        direction,
        distance: Math.abs(distance),
        velocity
    };
};