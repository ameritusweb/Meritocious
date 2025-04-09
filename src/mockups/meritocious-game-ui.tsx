import React from 'react';
import { MessageSquare, GitFork, Star, Shield, Zap, Brain } from 'lucide-react';

// Individual petal for the merit bar
const MeritPetal = ({ filled, partial }) => (
  <div className="relative w-6 h-6 transform hover:scale-110 transition-transform">
    <svg viewBox="0 0 24 24" className="w-full h-full">
      <path
        d="M12 3 L16 8 L16 15 L12 20 L8 15 L8 8 Z"
        className={`transform-origin-center ${
          filled 
            ? 'fill-teal-500 stroke-teal-300' 
            : partial 
              ? 'fill-teal-500/30 stroke-teal-300/30' 
              : 'fill-gray-800 stroke-gray-700'
        }`}
        strokeWidth="1"
      />
    </svg>
  </div>
);

// Merit bar component
const MeritBar = ({ score, maxScore = 10 }) => {
  const fullPetals = Math.floor(score);
  const partial = score % 1;

  return (
    <div className="relative">
      {/* Background glow effect */}
      <div className="absolute inset-0 bg-teal-500/5 blur-xl" />
      
      {/* Main bar container */}
      <div className="relative flex items-center gap-1 p-2 bg-gray-900/80 rounded-lg border border-gray-700/50">
        <Brain className="w-5 h-5 text-teal-400 mr-2" />
        
        {/* Petals container */}
        <div className="flex gap-1">
          {[...Array(maxScore)].map((_, i) => (
            <MeritPetal
              key={i}
              filled={i < fullPetals}
              partial={i === fullPetals && partial > 0}
            />
          ))}
        </div>
        
        {/* Numeric display */}
        <div className="ml-3 text-lg font-game text-teal-300">
          {score.toFixed(2)}
        </div>
      </div>
    </div>
  );
};

// Game-style post card
const PostCard = ({ title, author, content, meritScore, stats }) => (
  <div className="relative p-1 bg-gradient-to-r from-teal-500/20 to-blue-500/20 rounded-lg overflow-hidden">
    {/* Inner content with game-style border */}
    <div className="p-6 bg-gray-900/95 rounded-lg border border-gray-700/50">
      {/* Title section */}
      <div className="flex items-center gap-4 mb-4">
        <div className="flex-1">
          <h3 className="text-xl font-bold text-teal-300">{title}</h3>
          <div className="text-sm text-gray-400">by {author}</div>
        </div>
        <MeritBar score={meritScore} maxScore={10} />
      </div>

      {/* Content */}
      <div className="mb-6 text-gray-300">{content}</div>

      {/* Stats grid */}
      <div className="grid grid-cols-3 gap-4">
        {stats.map((stat, i) => (
          <div key={i} className="flex items-center gap-2 p-2 bg-gray-800/50 rounded-lg">
            {stat.icon}
            <div>
              <div className="text-sm text-gray-400">{stat.label}</div>
              <div className="text-lg font-bold text-gray-200">{stat.value}</div>
            </div>
          </div>
        ))}
      </div>
    </div>
  </div>
);

// Main view
const GameView = () => {
  const postStats = [
    { icon: <GitFork className="w-5 h-5 text-teal-400" />, label: "Forks", value: "12" },
    { icon: <Shield className="w-5 h-5 text-blue-400" />, label: "Citations", value: "8" },
    { icon: <Zap className="w-5 h-5 text-yellow-400" />, label: "Impact", value: "85" },
  ];

  return (
    <div className="min-h-screen bg-gray-900 p-8">
      {/* Top navigation bar */}
      <div className="flex items-center justify-between mb-8 p-4 bg-gray-800/50 rounded-lg border border-gray-700/50">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-teal-500/10 rounded-lg">
            <Brain className="w-8 h-8 text-teal-400" />
          </div>
          <span className="text-2xl font-bold text-teal-300">MERITOCIOUS</span>
        </div>
        
        <div className="flex gap-4">
          <button className="px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded-lg text-gray-300 transition-colors">
            Quests
          </button>
          <button className="px-4 py-2 bg-teal-500 hover:bg-teal-400 rounded-lg text-white transition-colors">
            New Fork
          </button>
        </div>
      </div>

      {/* Content grid */}
      <div className="grid gap-6">
        <PostCard
          title="Recursive Self-Improvement Theory"
          author="AIResearcher"
          content="I've been thinking about how we might approach the recursive self-improvement problem from a different angle. Instead of focusing on direct control mechanisms, what if we considered evolutionary stable strategies from game theory?"
          meritScore={8.75}
          stats={postStats}
        />
        
        <PostCard
          title="Multi-Agent Systems Analysis"
          author="SystemsTheorist"
          content="Building on the game theory approach above, we could model this as a cooperative multi-agent system where each iteration of the AI is treated as a separate agent with aligned but not identical incentives."
          meritScore={7.25}
          stats={postStats}
        />
        
        <PostCard
          title="Game Theory Edge Cases"
          author="SafetyFirst"
          content="While the game theoretic approach is interesting, we need to consider several edge cases where traditional Nash equilibria might not hold in a recursive improvement scenario."
          meritScore={6.50}
          stats={postStats}
        />
      </div>
    </div>
  );
};

export default GameView;