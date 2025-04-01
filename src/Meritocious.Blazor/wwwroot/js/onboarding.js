﻿let resizeHandler = null;

export function initOnboardingPage() {
    // Screen size detection
    window.isSmallScreen = function () {
        return window.innerWidth < 768;
    };

    // Save onboarding progress
    window.saveOnboardingProgress = function (step, data) {
        localStorage.setItem('onboardingStep', step);
        localStorage.setItem('onboardingData', JSON.stringify(data));
    };

    // Load onboarding progress
    window.loadOnboardingProgress = function () {
        const step = localStorage.getItem('onboardingStep');
        const data = localStorage.getItem('onboardingData');
        return {
            step: step ? parseInt(step) : 0,
            data: data ? JSON.parse(data) : null
        };
    };

    // Clear onboarding progress
    window.clearOnboardingProgress = function () {
        localStorage.removeItem('onboardingStep');
        localStorage.removeItem('onboardingData');
    };

    resizeHandler = function () {
        if (window.onScreenSizeChanged) {
            window.onScreenSizeChanged(window.innerWidth < 768);
        }
    };

    // Handle resize for responsive steps
    window.addEventListener('resize', resizeHandler);

    // Smooth scroll to next section
    window.scrollToNextSection = function () {
        const nextSection = document.querySelector('.ant-steps-item-wait');
        if (nextSection) {
            nextSection.scrollIntoView({ behavior: 'smooth' });
        }
    };
}

export function cleanupOnboardingPage() {
    if (resizeHandler) {
        window.removeEventListener('resize', resizeHandler);
        resizeHandler = null;
    }
}