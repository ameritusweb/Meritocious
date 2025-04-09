import React, { useState } from 'react';
import { Brain, GitFork, Star, MessageSquare, Heart, ArrowRight, UserPlus, LogIn } from 'lucide-react';

// Previous FloatingPetal component remains the same
const FLOWERS = ['🌸', '🌺', '🌼', '🌻', '💮', '🏵️', '⚘', '✿'];

const FloatingPetal = ({ style }) => {
  const randomFlower = FLOWERS[Math.floor(Math.random() * FLOWERS.length)];
  return (
    <div className="absolute select-none animate-float transition-opacity"
         style={{ fontSize: `${1 + Math.random() * 1.5}rem`, ...style }}>
      {randomFlower}
    </div>
  );
};

// Merit bar component for demonstration
const MeritBar = ({ score }) => (
  <div className="flex items-center gap-1">
    {[...Array(5)].map((_, i) => (
      <Heart key={i} 
            className={`w-5 h-5 ${i < score ? 'text-teal-400 fill-current' : 'text-gray-600'}`} />
    ))}
  </div>
);

const LearnMoreContent = () => (
  <div className="max-w-4xl mx-auto p-8">
    <div className="space-y-16">
      {/* Section 1: Merit System */}
      <section className="bg-gray-800/30 backdrop-blur-sm rounded-xl p-8 border border-gray-700/50">
        <h2 className="text-2xl font-bold text-white mb-6">Merit-Based Discussion</h2>
        <div className="flex gap-8 items-center">
          <div className="flex-1">
            <p className="text-gray-300 mb-4">
              Unlike traditional platforms that rely on likes and upvotes, Meritocious uses AI to evaluate contributions based on:
            </p>
            <div className="grid grid-cols-2 gap-4">
              {['Clarity', 'Novelty', 'Insight', 'Civility'].map(quality => (
                <div key={quality} className="flex items-center gap-2 text-gray-300">
                  <Star className="w-4 h-4 text-teal-400" />
                  <span>{quality}</span>
                </div>
              ))}
            </div>
          </div>
          <div className="flex flex-col items-center gap-2">
            <MeritBar score={4} />
            <span className="text-sm text-gray-400">Merit Score</span>
          </div>
        </div>
      </section>

      {/* Section 2: Forking */}
      <section className="bg-gray-800/30 backdrop-blur-sm rounded-xl p-8 border border-gray-700/50">
        <h2 className="text-2xl font-bold text-white mb-6">Fork and Evolve Ideas</h2>
        <div className="grid grid-cols-3 gap-8">
          <div className="space-y-3">
            <GitFork className="w-8 h-8 text-teal-400" />
            <h3 className="text-lg font-semibold text-white">Fork</h3>
            <p className="text-gray-300">Branch off existing discussions to explore new angles</p>
          </div>
          <div className="space-y-3">
            <MessageSquare className="w-8 h-8 text-teal-400" />
            <h3 className="text-lg font-semibold text-white">Discuss</h3>
            <p className="text-gray-300">Engage in thoughtful conversation with others</p>
          </div>
          <div className="space-y-3">
            <Star className="w-8 h-8 text-teal-400" />
            <h3 className="text-lg font-semibold text-white">Earn</h3>
            <p className="text-gray-300">Gain recognition for valuable contributions</p>
          </div>
        </div>
      </section>

      {/* Section 3: Call to Action */}
      <section className="text-center">
        <h2 className="text-2xl font-bold text-white mb-4">Ready to join the conversation?</h2>
        <div className="flex justify-center gap-4">
          <button className="px-6 py-3 bg-teal-500 text-white rounded-lg font-medium hover:bg-teal-400 transition-colors">
            Sign Up Now
          </button>
          <button className="px-6 py-3 bg-gray-700 text-white rounded-lg font-medium hover:bg-gray-600 transition-colors">
            Explore More
          </button>
        </div>
      </section>
    </div>
  </div>
);

const App = () => {
  const [showLearnMore, setShowLearnMore] = useState(false);

  // Generate positions for petals
  const petals = [...Array(25)].map(() => ({
    left: `${Math.random() * 100}%`,
    top: `${Math.random() * 100}%`,
    animationDelay: `${Math.random() * 5}s`,
    animationDuration: `${10 + Math.random() * 10}s`,
    opacity: 0.15 + Math.random() * 0.15,
    transform: `rotate(${Math.random() * 360}deg)`,
  }));

  return (
    <div className="min-h-screen bg-gray-900 text-white">
      {/* Auth buttons in top right */}
      <div className="absolute top-4 right-4 z-50 flex gap-4">
        <button className="px-4 py-2 bg-gray-800/50 backdrop-blur-sm rounded-lg text-gray-300 hover:bg-gray-700/50 flex items-center gap-2">
          <LogIn className="w-4 h-4" />
          <span>Login</span>
        </button>
        <button className="px-4 py-2 bg-teal-500 rounded-lg text-white hover:bg-teal-400 flex items-center gap-2">
          <UserPlus className="w-4 h-4" />
          <span>Sign Up</span>
        </button>
      </div>

      {/* Background petals */}
      <div className="fixed inset-0 overflow-hidden">
        {petals.map((style, i) => (
          <FloatingPetal key={i} style={style} />
        ))}
      </div>

      {/* Content */}
      {!showLearnMore ? (
        <div className="relative flex items-center justify-center min-h-screen backdrop-blur-sm">
          <div className="text-center">
            <div className="mb-12">
              <Brain className="w-24 h-24 mx-auto text-teal-400 mb-6" />
              <h1 className="text-4xl font-bold text-white mb-4">Meritocious</h1>
              <p className="text-xl text-gray-300">Where ideas evolve through merit</p>
            </div>

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

            <button
              onClick={() => setShowLearnMore(true)}
              className="px-8 py-3 bg-gradient-to-r from-teal-500 to-teal-400 text-white 
                       rounded-lg text-lg font-medium flex items-center gap-2 mx-auto
                       hover:from-teal-400 hover:to-teal-300 transition-all duration-300
                       hover:shadow-xl hover:shadow-teal-500/25"
            >
              Learn More
              <ArrowRight className="w-5 h-5" />
            </button>
          </div>
        </div>
      ) : (
        <LearnMoreContent />
      )}
    </div>
  );
};

export default App;