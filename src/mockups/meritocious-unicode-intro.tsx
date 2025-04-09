import React, { useState, useEffect } from 'react';
import { Brain, GitFork, Star, MessageSquare } from 'lucide-react';

// Array of flower-related Unicode characters
const FLOWERS = ['🌸', '🌺', '🌼', '🌻', '💮', '🏵️', '⚘', '✿'];

const FloatingPetal = ({ style }) => {
  const randomFlower = FLOWERS[Math.floor(Math.random() * FLOWERS.length)];
  
  return (
    <div 
      className="absolute select-none animate-float transition-opacity"
      style={{
        fontSize: `${1 + Math.random() * 1.5}rem`,
        ...style
      }}
    >
      {randomFlower}
    </div>
  );
};

const FloatingPetalsIntro = ({ onComplete }) => {
  const [showButton, setShowButton] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => setShowButton(true), 1000);
    return () => clearTimeout(timer);
  }, []);

  // Generate positions for petals
  const petals = [...Array(25)].map((_, i) => ({
    left: `${Math.random() * 100}%`,
    top: `${Math.random() * 100}%`,
    animationDelay: `${Math.random() * 5}s`,
    animationDuration: `${10 + Math.random() * 10}s`,
    opacity: 0.15 + Math.random() * 0.15,
    transform: `rotate(${Math.random() * 360}deg)`,
  }));

  return (
    <div className="relative h-screen bg-gray-900 overflow-hidden">
      {/* Floating petals background */}
      <div className="absolute inset-0">
        {petals.map((style, i) => (
          <FloatingPetal key={i} style={style} />
        ))}
      </div>

      {/* Content overlay with slight glass effect */}
      <div className="relative flex items-center justify-center h-full backdrop-blur-sm">
        <div className="text-center">
          {/* Main content */}
          <div className="mb-12">
            <div className="relative">
              <Brain className="w-24 h-24 mx-auto text-teal-400 mb-6" />
              {/* Subtle glow effect behind the brain icon */}
              <div className="absolute inset-0 bg-teal-500/20 blur-xl -z-10" />
            </div>
            <h1 className="text-4xl font-bold text-white mb-4">Meritocious</h1>
            <p className="text-xl text-gray-300">Where ideas evolve through merit</p>
          </div>

          {/* Feature icons */}
          <div className="flex justify-center gap-12 mb-12">
            <div className="text-center p-4 rounded-lg bg-gray-800/30 backdrop-blur-sm">
              <GitFork className="w-8 h-8 text-teal-400 mx-auto mb-2" />
              <p className="text-gray-300">Fork Ideas</p>
            </div>
            <div className="text-center p-4 rounded-lg bg-gray-800/30 backdrop-blur-sm">
              <Star className="w-8 h-8 text-teal-400 mx-auto mb-2" />
              <p className="text-gray-300">Earn Merit</p>
            </div>
            <div className="text-center p-4 rounded-lg bg-gray-800/30 backdrop-blur-sm">
              <MessageSquare className="w-8 h-8 text-teal-400 mx-auto mb-2" />
              <p className="text-gray-300">Join Discussions</p>
            </div>
          </div>

          {/* Get Started button */}
          <div className={`transition-opacity duration-1000 ${showButton ? 'opacity-100' : 'opacity-0'}`}>
            <button
              onClick={onComplete}
              className="px-8 py-3 bg-gradient-to-r from-teal-500 to-teal-400 text-white 
                       rounded-lg text-lg font-medium shadow-lg
                       hover:from-teal-400 hover:to-teal-300
                       transform hover:scale-105 transition-all duration-300
                       hover:shadow-teal-500/25 hover:shadow-xl"
            >
              Get Started
            </button>
          </div>
        </div>
      </div>

      {/* Subtle vignette overlay */}
      <div className="absolute inset-0 bg-gradient-to-b from-gray-900/50 to-transparent pointer-events-none" />
    </div>
  );
};

// Add custom animation keyframes to the existing Tailwind classes
const style = document.createElement('style');
style.textContent = `
  @keyframes float {
    0% {
      transform: translateY(0) rotate(0deg);
    }
    33% {
      transform: translateY(-50px) rotate(120deg);
    }
    67% {
      transform: translateY(50px) rotate(240deg);
    }
    100% {
      transform: translateY(0) rotate(360deg);
    }
  }

  .animate-float {
    animation: float linear infinite;
  }
`;
document.head.appendChild(style);

// Demo wrapper
const App = () => {
  const handleComplete = () => {
    console.log('Starting the application...');
  };

  return <FloatingPetalsIntro onComplete={handleComplete} />;
};

export default App;