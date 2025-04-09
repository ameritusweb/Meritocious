import React, { useState } from 'react';
import { Search, Filter, Heart, GitFork, MessageSquare, 
         Star, TrendingUp, Clock, X, ChevronDown, Tag } from 'lucide-react';

// Merit score component
const MeritBar = ({ score }) => (
  <div className="flex items-center gap-1">
    {[...Array(5)].map((_, i) => (
      <Heart key={i} 
            className={`w-4 h-4 ${i < score ? 'text-teal-400 fill-current' : 'text-gray-600'}`} />
    ))}
    <span className="ml-2 text-sm text-teal-400">{score.toFixed(1)}</span>
  </div>
);

// Search result card
const SearchResult = ({ title, author, excerpt, merit, type, timestamp, tags, forks, replies }) => (
  <div className="bg-gray-800/50 backdrop-blur-sm rounded-lg p-6 border border-gray-700/50 hover:border-teal-500/30 transition-colors">
    <div className="flex items-start justify-between mb-3">
      <div>
        <h3 className="text-lg font-medium text-white hover:text-teal-400 transition-colors cursor-pointer">
          {title}
        </h3>
        <div className="flex items-center gap-3 mt-1 text-sm text-gray-400">
          <span>by {author}</span>
          <span className="px-2 py-0.5 bg-gray-700/50 rounded text-teal-400">{type}</span>
          <span className="flex items-center gap-1">
            <Clock size={14} />
            {timestamp}
          </span>
        </div>
      </div>
      <MeritBar score={merit} />
    </div>
    
    <p className="text-gray-300 mb-4">{excerpt}</p>
    
    <div className="flex items-center justify-between">
      <div className="flex items-center gap-2">
        {tags.map(tag => (
          <span key={tag} className="px-2 py-1 bg-gray-700/30 rounded-full text-sm text-gray-300 hover:bg-gray-600/30 cursor-pointer">
            #{tag}
          </span>
        ))}
      </div>
      
      <div className="flex items-center gap-4 text-gray-400">
        <div className="flex items-center gap-2">
          <GitFork size={16} />
          <span>{forks}</span>
        </div>
        <div className="flex items-center gap-2">
          <MessageSquare size={16} />
          <span>{replies}</span>
        </div>
      </div>
    </div>
  </div>
);

// Filter chip component
const FilterChip = ({ label, onRemove }) => (
  <div className="flex items-center gap-2 px-3 py-1.5 bg-teal-500/20 text-teal-400 rounded-full text-sm">
    <span>{label}</span>
    <button onClick={onRemove} className="hover:text-teal-300">
      <X size={14} />
    </button>
  </div>
);

// Main search page
const SearchPage = () => {
  const [activeFilters, setActiveFilters] = useState(['Merit > 4.0', 'This Month']);
  
  const mockResults = [
    {
      title: "The Future of Distributed Intelligence",
      author: "AIResearcher",
      excerpt: "Exploring how decentralized AI systems might evolve to create emergent intelligence patterns beyond our current frameworks.",
      merit: 4.8,
      type: "Original Post",
      timestamp: "2 days ago",
      tags: ["AI", "Distributed Systems", "Evolution"],
      forks: 12,
      replies: 24
    },
    {
      title: "Re: Distributed Intelligence - A Game Theory Perspective",
      author: "GameTheorist",
      excerpt: "Building on the original post with insights from Nash equilibrium in multi-agent systems.",
      merit: 4.5,
      type: "Fork",
      timestamp: "1 day ago",
      tags: ["Game Theory", "AI", "Multi-Agent"],
      forks: 8,
      replies: 15
    }
  ];

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      <div className="max-w-6xl mx-auto">
        {/* Search Header */}
        <div className="mb-8">
          <div className="relative">
            <input
              type="text"
              placeholder="Search ideas, discussions, and forks..."
              className="w-full px-12 py-4 bg-gray-800/50 border border-gray-700/50 rounded-lg
                       text-white placeholder-gray-400 focus:outline-none focus:border-teal-500/50
                       focus:ring-1 focus:ring-teal-500/50 backdrop-blur-sm text-lg"
            />
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          </div>
        </div>

        {/* Filters Bar */}
        <div className="flex items-center justify-between mb-6 bg-gray-800/30 rounded-lg p-4 backdrop-blur-sm">
          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <Filter size={16} className="text-gray-400" />
              <span className="text-gray-400">Filters:</span>
            </div>
            
            <div className="flex items-center gap-2">
              {activeFilters.map((filter, i) => (
                <FilterChip 
                  key={i} 
                  label={filter} 
                  onRemove={() => setActiveFilters(prev => prev.filter(f => f !== filter))} 
                />
              ))}
            </div>
          </div>

          <div className="flex items-center gap-4">
            <div className="flex items-center gap-2">
              <span className="text-gray-400">Sort by:</span>
              <button className="flex items-center gap-2 px-3 py-1.5 bg-gray-700/50 rounded-lg text-gray-300">
                Merit Score
                <ChevronDown size={16} />
              </button>
            </div>
            
            <button className="px-4 py-2 bg-gray-700/50 rounded-lg text-gray-300 hover:bg-gray-600/50 flex items-center gap-2">
              Advanced Filters
            </button>
          </div>
        </div>

        {/* Quick Filters */}
        <div className="grid grid-cols-4 gap-4 mb-8">
          {[
            { icon: Star, label: "High Merit", value: "450+ posts" },
            { icon: TrendingUp, label: "Trending", value: "24 new" },
            { icon: GitFork, label: "Most Forked", value: "120+ forks" },
            { icon: Tag, label: "Popular Tags", value: "View all" }
          ].map((filter, i) => (
            <button key={i} className="flex items-center gap-4 p-4 bg-gray-800/30 rounded-lg hover:bg-gray-700/30 transition-colors">
              <div className="p-2 bg-gray-700/50 rounded">
                <filter.icon className="w-5 h-5 text-teal-400" />
              </div>
              <div className="text-left">
                <div className="text-gray-300">{filter.label}</div>
                <div className="text-sm text-gray-400">{filter.value}</div>
              </div>
            </button>
          ))}
        </div>

        {/* Search Results */}
        <div className="space-y-4">
          <div className="text-gray-400 mb-6">Showing 127 results</div>
          
          {mockResults.map((result, i) => (
            <SearchResult key={i} {...result} />
          ))}
        </div>
      </div>
    </div>
  );
};

export default SearchPage;