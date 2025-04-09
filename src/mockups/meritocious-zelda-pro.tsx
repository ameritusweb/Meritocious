import React, { useState, useEffect } from 'react';
import { Heart, GitFork, MessageSquare, Star, Brain, MapPin } from 'lucide-react';

// Zelda-inspired merit bar that remains professional
const MeritBar = ({ score, maxScore = 10 }) => {
  const fullHearts = Math.floor(score);
  const partialHeart = score % 1;
  
  return (
    <div className="flex items-center gap-2 p-2 bg-gray-800/60 rounded-lg backdrop-blur-sm">
      <div className="flex gap-1">
        {[...Array(maxScore)].map((_, i) => (
          <div key={i} className="relative w-5 h-5 transform hover:scale-110 transition-transform">
            {/* Empty heart container */}
            <Heart 
              className={`absolute w-full h-full stroke-2 ${
                i < fullHearts ? 'text-teal-500' : 
                (i === fullHearts && partialHeart > 0) ? 'text-teal-500/50' : 
                'text-gray-600'
              }`}
              fill={i < fullHearts ? 'currentColor' : 'none'}
            />
            
            {/* Partial fill effect */}
            {i === fullHearts && partialHeart > 0 && (
              <div 
                className="absolute bottom-0 bg-teal-500 opacity-50"
                style={{ 
                  width: '100%',
                  height: `${partialHeart * 100}%`,
                  borderRadius: '0 0 4px 4px'
                }}
              />
            )}
          </div>
        ))}
      </div>
      <span className="text-sm font-medium text-teal-300">{score.toFixed(2)}</span>
    </div>
  );
};

// Professional notification for new forks
const ForkNotification = ({ title, author }) => {
  const [show, setShow] = useState(true);
  
  return (
    <div className={`fixed top-4 right-4 transition-all duration-500 transform ${
      show ? 'translate-x-0 opacity-100' : 'translate-x-full opacity-0'
    }`}>
      <div className="p-4 bg-gray-800/90 backdrop-blur-sm rounded-lg border border-teal-500/30 shadow-lg">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-teal-500/20 rounded-full">
            <GitFork className="w-5 h-5 text-teal-400" />
          </div>
          <div>
            <div className="text-sm font-medium text-gray-300">New Fork Available</div>
            <div className="text-xs text-gray-400">{title} by {author}</div>
          </div>
        </div>
      </div>
    </div>
  );
};

// Smooth intro animation component
const IntroSequence = ({ onComplete }) => {
  const [step, setStep] = useState(0);
  
  useEffect(() => {
    const timer = setTimeout(() => {
      if (step < 3) {
        setStep(step + 1);
      } else {
        onComplete();
      }
    }, 2000);
    return () => clearTimeout(timer);
  }, [step]);
  
  return (
    <div className="fixed inset-0 flex items-center justify-center bg-gray-900">
      <div className="relative">
        {/* Floating clouds background effect */}
        <div className="absolute inset-0 overflow-hidden">
          <div className="absolute w-96 h-96 bg-teal-500/5 rounded-full blur-3xl animate-float-slow" 
               style={{ top: '10%', left: '20%' }} />
          <div className="absolute w-96 h-96 bg-blue-500/5 rounded-full blur-3xl animate-float-slower" 
               style={{ top: '40%', right: '20%' }} />
        </div>
        
        {/* Intro content */}
        <div className={`transition-all duration-1000 ${
          step === 0 ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-10'
        }`}>
          <Brain className="w-24 h-24 text-teal-400 mb-6" />
          <h1 className="text-4xl font-bold text-white text-center">Welcome to Meritocious</h1>
        </div>
        
        {step === 1 && (
          <div className="text-xl text-gray-300 text-center animate-fade-in">
            Where ideas evolve through merit
          </div>
        )}
        
        {step === 2 && (
          <div className="flex flex-col items-center animate-fade-in">
            <MeritBar score={8.5} maxScore={5} />
            <div className="mt-4 text-gray-400">Track your impact</div>
          </div>
        )}
      </div>
    </div>
  );
};

// World map inspired post navigation
const PostMap = ({ posts }) => {
  return (
    <div className="relative p-8 bg-gray-800/30 rounded-xl backdrop-blur-sm">
      <div className="absolute inset-0 bg-gray-900/50 rounded-xl" />
      
      {/* Post nodes with connecting lines */}
      <div className="relative grid gap-8">
        {posts.map((post, i) => (
          <div key={i} className="relative">
            {/* Connection line */}
            {i > 0 && (
              <div className="absolute top-1/2 -left-8 w-8 h-px bg-gradient-to-r from-teal-500/20 to-teal-500" />
            )}
            
            {/* Post node */}
            <div className="relative p-6 bg-gray-800/80 rounded-lg border border-gray-700/50 hover:border-teal-500/50 transition-colors">
              <div className="absolute -left-3 top-1/2 transform -translate-y-1/2">
                <MapPin className="w-6 h-6 text-teal-400" />
              </div>
              
              <div className="flex items-start justify-between">
                <div>
                  <h3 className="text-lg font-medium text-white">{post.title}</h3>
                  <div className="text-sm text-gray-400">{post.author}</div>
                </div>
                <MeritBar score={post.merit} maxScore={5} />
              </div>
              
              <div className="mt-4 text-gray-300">{post.excerpt}</div>
              
              <div className="mt-4 flex gap-4">
                <button className="flex items-center gap-2 px-3 py-1.5 bg-gray-700/50 rounded-md text-sm text-gray-300 hover:bg-gray-600/50">
                  <GitFork className="w-4 h-4" />
                  <span>{post.forks}</span>
                </button>
                <button className="flex items-center gap-2 px-3 py-1.5 bg-gray-700/50 rounded-md text-sm text-gray-300 hover:bg-gray-600/50">
                  <MessageSquare className="w-4 h-4" />
                  <span>{post.replies}</span>
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

// Main application view
const App = () => {
  const [showIntro, setShowIntro] = useState(true);
  
  const samplePosts = [
    {
      title: "Original Thesis",
      author: "AIResearcher",
      merit: 8.5,
      excerpt: "Initial exploration of recursive self-improvement...",
      forks: 12,
      replies: 24
    },
    {
      title: "Alternative Approach",
      author: "SystemsTheorist",
      merit: 7.25,
      excerpt: "A different perspective on the original thesis...",
      forks: 8,
      replies: 16
    },
    {
      title: "Synthesis",
      author: "SafetyFirst",
      merit: 9.0,
      excerpt: "Combining both approaches into a unified theory...",
      forks: 15,
      replies: 30
    }
  ];

  return (
    <div className="min-h-screen bg-gray-900 text-white">
      {showIntro ? (
        <IntroSequence onComplete={() => setShowIntro(false)} />
      ) : (
        <div className="max-w-4xl mx-auto p-8">
          <h1 className="text-3xl font-bold mb-8">Idea Evolution Map</h1>
          <PostMap posts={samplePosts} />
          <ForkNotification 
            title="New perspective on RSI"
            author="AIResearcher"
          />
        </div>
      )}
    </div>
  );
};

export default App;