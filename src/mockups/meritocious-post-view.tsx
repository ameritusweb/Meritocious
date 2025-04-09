import React, { useState } from 'react';
import { 
  Heart, GitFork, MessageSquare, Star, Share2, Bookmark,
  Users, ArrowRight, Clock, Tag, Brain, TreeDeciduous
} from 'lucide-react';

// Merit bar with hover details
const MeritBar = ({ score, details }) => {
  const [showDetails, setShowDetails] = useState(false);
  
  return (
    <div className="relative">
      <div 
        className="flex items-center gap-2 p-2 bg-gray-800/50 rounded-lg cursor-help"
        onMouseEnter={() => setShowDetails(true)}
        onMouseLeave={() => setShowDetails(false)}
      >
        <div className="flex gap-1">
          {[...Array(5)].map((_, i) => (
            <Heart 
              key={i}
              className={`w-5 h-5 ${i < score ? 'text-teal-400 fill-current' : 'text-gray-600'}`}
            />
          ))}
        </div>
        <span className="text-teal-400 font-medium">{score.toFixed(1)}</span>
      </div>

      {/* Merit details tooltip */}
      {showDetails && (
        <div className="absolute top-full mt-2 w-64 p-4 bg-gray-800/95 backdrop-blur-sm rounded-lg shadow-xl z-10">
          <h4 className="text-sm font-medium text-white mb-3">Merit Breakdown</h4>
          {Object.entries(details).map(([key, value]) => (
            <div key={key} className="flex items-center justify-between mb-2">
              <span className="text-sm text-gray-400">{key}</span>
              <span className="text-sm text-teal-400">{value.toFixed(1)}</span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

// Related fork preview
const ForkPreview = ({ title, author, merit, excerpt }) => (
  <div className="p-4 bg-gray-800/30 rounded-lg border border-gray-700/50 hover:border-teal-500/30 transition-colors">
    <div className="flex items-start justify-between mb-2">
      <div>
        <h3 className="text-white font-medium">{title}</h3>
        <p className="text-sm text-gray-400">by {author}</p>
      </div>
      <MeritBar score={merit} details={{}} />
    </div>
    <p className="text-sm text-gray-300">{excerpt}</p>
  </div>
);

const PostView = () => {
  const post = {
    title: "Recursive Self-Improvement: A New Framework",
    author: "AIResearcher",
    timestamp: "2 days ago",
    content: `I've been thinking about how we might approach the recursive self-improvement problem from a different angle. Instead of focusing on direct control mechanisms, what if we considered evolutionary stable strategies from game theory?

The key insight is that we can potentially create systems that are inherently stable through their reward functions, rather than trying to impose external constraints.

This approach has several advantages:
1. It aligns with natural selection principles
2. It's more robust to environmental changes
3. It could potentially scale better than direct control

What do you think about this perspective? I'm particularly interested in hearing thoughts on how this might interact with current alignment approaches.`,
    merit: 4.8,
    meritDetails: {
      Clarity: 4.9,
      Novelty: 4.7,
      Impact: 4.8,
      Civility: 5.0,
      Depth: 4.6
    },
    tags: ["AI Safety", "Game Theory", "Evolution", "Systems"],
    stats: {
      forks: 12,
      replies: 24,
      viewers: 156
    }
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white">
      <div className="max-w-5xl mx-auto p-8">
        {/* Post Header */}
        <div className="mb-8">
          <div className="flex items-start justify-between mb-4">
            <div>
              <h1 className="text-3xl font-bold text-white mb-2">{post.title}</h1>
              <div className="flex items-center gap-4 text-gray-400">
                <span className="flex items-center gap-2">
                  <Brain className="w-4 h-4" />
                  {post.author}
                </span>
                <span className="flex items-center gap-2">
                  <Clock className="w-4 h-4" />
                  {post.timestamp}
                </span>
              </div>
            </div>
            <MeritBar score={post.merit} details={post.meritDetails} />
          </div>

          {/* Tags */}
          <div className="flex items-center gap-2 mb-6">
            {post.tags.map(tag => (
              <span key={tag} className="px-3 py-1 bg-gray-800/50 rounded-full text-sm text-teal-400">
                {tag}
              </span>
            ))}
          </div>

          {/* Stats Bar */}
          <div className="flex items-center gap-6 p-4 bg-gray-800/30 rounded-lg">
            <div className="flex items-center gap-2 text-gray-400">
              <GitFork className="w-4 h-4" />
              <span>{post.stats.forks} Forks</span>
            </div>
            <div className="flex items-center gap-2 text-gray-400">
              <MessageSquare className="w-4 h-4" />
              <span>{post.stats.replies} Replies</span>
            </div>
            <div className="flex items-center gap-2 text-gray-400">
              <Users className="w-4 h-4" />
              <span>{post.stats.viewers} Viewers</span>
            </div>
            <div className="flex-1 flex justify-end gap-2">
              <button className="p-2 text-gray-400 hover:text-white">
                <Share2 className="w-5 h-5" />
              </button>
              <button className="p-2 text-gray-400 hover:text-white">
                <Bookmark className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="mb-8">
          <div className="prose prose-invert max-w-none">
            {post.content.split('\n\n').map((paragraph, i) => (
              <p key={i} className="mb-4 text-gray-300 leading-relaxed">
                {paragraph}
              </p>
            ))}
          </div>
        </div>

        {/* Action Bar */}
        <div className="flex items-center justify-between mb-8 p-4 bg-gray-800/30 rounded-lg">
          <div className="flex items-center gap-4">
            <button className="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-400 flex items-center gap-2">
              <GitFork className="w-4 h-4" />
              Fork This Idea
            </button>
            <button className="px-4 py-2 bg-gray-700 text-gray-300 rounded-lg hover:bg-gray-600 flex items-center gap-2">
              <MessageSquare className="w-4 h-4" />
              Add to Discussion
            </button>
          </div>
          <button className="flex items-center gap-2 text-gray-400 hover:text-teal-400">
            <TreeDeciduous className="w-4 h-4" />
            View Fork Tree
          </button>
        </div>

        {/* Related Forks */}
        <div className="mb-8">
          <h2 className="text-xl font-bold mb-4">Notable Forks</h2>
          <div className="grid grid-cols-2 gap-4">
            <ForkPreview 
              title="Game Theory Applications in RSI"
              author="GameTheorist"
              merit={4.5}
              excerpt="Extending the evolutionary strategy approach with specific game theoretic frameworks..."
            />
            <ForkPreview 
              title="Multi-Agent Stability Analysis"
              author="SystemsEngineer"
              merit={4.3}
              excerpt="Analyzing the stability properties of recursive improvement in multi-agent contexts..."
            />
          </div>
        </div>

        {/* Discussion Thread Preview */}
        <div>
          <h2 className="text-xl font-bold mb-4">Active Discussions</h2>
          <div className="space-y-4">
            {/* Add discussion thread components here */}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PostView;