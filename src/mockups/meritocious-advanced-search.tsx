import React, { useState } from 'react';
import { Search, Filter, Heart, GitFork, MessageSquare, X, ChevronDown, 
         Calendar, Tag, Users, Clock, ArrowRight, Sliders, RefreshCw } from 'lucide-react';

// Range Slider component
const RangeSlider = ({ min, max, value, onChange, label }) => (
  <div className="space-y-2">
    <div className="flex justify-between text-sm text-gray-400">
      <span>{label}</span>
      <span>{value.toFixed(1)}</span>
    </div>
    <input
      type="range"
      min={min}
      max={max}
      step={0.1}
      value={value}
      onChange={e => onChange(parseFloat(e.target.value))}
      className="w-full accent-teal-500 bg-gray-700"
    />
  </div>
);

// Filter Panel
const AdvancedFilterPanel = ({ isOpen, onClose }) => {
  const [meritScore, setMeritScore] = useState(4.0);
  const [selectedContentTypes, setSelectedContentTypes] = useState(['posts', 'forks']);
  const [selectedTimeRange, setSelectedTimeRange] = useState('month');
  
  const contentTypes = ['posts', 'forks', 'comments', 'discussions'];
  const timeRanges = [
    { id: 'day', label: 'Past 24 Hours' },
    { id: 'week', label: 'Past Week' },
    { id: 'month', label: 'Past Month' },
    { id: 'year', label: 'Past Year' },
    { id: 'all', label: 'All Time' }
  ];

  const toggleContentType = (type) => {
    setSelectedContentTypes(prev => 
      prev.includes(type) 
        ? prev.filter(t => t !== type)
        : [...prev, type]
    );
  };

  return (
    <div className={`fixed inset-y-0 right-0 w-96 bg-gray-800/95 backdrop-blur-xl shadow-2xl 
                    transform transition-transform duration-300 ease-in-out z-50
                    ${isOpen ? 'translate-x-0' : 'translate-x-full'}`}>
      {/* Header */}
      <div className="flex items-center justify-between p-6 border-b border-gray-700">
        <div className="flex items-center gap-2">
          <Sliders className="w-5 h-5 text-teal-400" />
          <h2 className="text-lg font-medium text-white">Advanced Filters</h2>
        </div>
        <button 
          onClick={onClose}
          className="text-gray-400 hover:text-white transition-colors"
        >
          <X size={20} />
        </button>
      </div>

      {/* Filter content */}
      <div className="p-6 space-y-8 h-[calc(100vh-70px)] overflow-y-auto">
        {/* Merit Score */}
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-gray-300">Merit Score</h3>
          <RangeSlider
            min={0}
            max={5}
            value={meritScore}
            onChange={setMeritScore}
            label="Minimum Merit Score"
          />
        </div>

        {/* Content Types */}
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-gray-300">Content Types</h3>
          <div className="grid grid-cols-2 gap-2">
            {contentTypes.map(type => (
              <button
                key={type}
                onClick={() => toggleContentType(type)}
                className={`px-4 py-2 rounded-lg text-sm capitalize transition-colors ${
                  selectedContentTypes.includes(type)
                    ? 'bg-teal-500 text-white'
                    : 'bg-gray-700/50 text-gray-300 hover:bg-gray-600/50'
                }`}
              >
                {type}
              </button>
            ))}
          </div>
        </div>

        {/* Time Range */}
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-gray-300">Time Range</h3>
          <div className="space-y-2">
            {timeRanges.map(range => (
              <button
                key={range.id}
                onClick={() => setSelectedTimeRange(range.id)}
                className={`w-full px-4 py-3 rounded-lg text-left transition-colors ${
                  selectedTimeRange === range.id
                    ? 'bg-teal-500 text-white'
                    : 'bg-gray-700/50 text-gray-300 hover:bg-gray-600/50'
                }`}
              >
                {range.label}
              </button>
            ))}
          </div>
        </div>

        {/* Additional Filters */}
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-gray-300">Additional Filters</h3>
          <div className="space-y-2">
            <button className="w-full px-4 py-3 bg-gray-700/50 rounded-lg text-gray-300 hover:bg-gray-600/50 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Tag size={16} />
                <span>Select Tags</span>
              </div>
              <ChevronDown size={16} />
            </button>
            <button className="w-full px-4 py-3 bg-gray-700/50 rounded-lg text-gray-300 hover:bg-gray-600/50 flex items-center justify-between">
              <div className="flex items-center gap-2">
                <Users size={16} />
                <span>Authors</span>
              </div>
              <ChevronDown size={16} />
            </button>
          </div>
        </div>

        {/* Popular Combinations */}
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-gray-300">Popular Combinations</h3>
          <div className="space-y-2">
            <button className="w-full px-4 py-3 bg-gray-700/50 rounded-lg text-left text-gray-300 hover:bg-gray-600/50">
              <div className="flex items-center justify-between">
                <span>Trending High Merit</span>
                <RefreshCw size={16} />
              </div>
              <div className="text-sm text-gray-400 mt-1">Merit > 4.5, Past Week, Most Forks</div>
            </button>
            <button className="w-full px-4 py-3 bg-gray-700/50 rounded-lg text-left text-gray-300 hover:bg-gray-600/50">
              <div className="flex items-center justify-between">
                <span>Active Discussions</span>
                <RefreshCw size={16} />
              </div>
              <div className="text-sm text-gray-400 mt-1">Merit > 3.5, Past Day, Most Replies</div>
            </button>
          </div>
        </div>
      </div>

      {/* Footer Actions */}
      <div className="absolute bottom-0 left-0 right-0 p-6 bg-gray-800/95 border-t border-gray-700">
        <div className="flex gap-4">
          <button className="flex-1 px-4 py-3 bg-gray-700 rounded-lg text-gray-300 hover:bg-gray-600">
            Reset
          </button>
          <button 
            onClick={onClose}
            className="flex-1 px-4 py-3 bg-teal-500 rounded-lg text-white hover:bg-teal-400 flex items-center justify-center gap-2"
          >
            Apply Filters
            <ArrowRight size={16} />
          </button>
        </div>
      </div>
    </div>
  );
};

// Modified SearchPage component to include the filter panel
const SearchPage = () => {
  const [isFilterPanelOpen, setIsFilterPanelOpen] = useState(false);

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      {/* Previous search page content remains the same */}
      <button 
        onClick={() => setIsFilterPanelOpen(true)}
        className="px-4 py-2 bg-gray-700/50 rounded-lg text-gray-300 hover:bg-gray-600/50 flex items-center gap-2"
      >
        <Filter size={16} />
        Advanced Filters
      </button>

      <AdvancedFilterPanel 
        isOpen={isFilterPanelOpen}
        onClose={() => setIsFilterPanelOpen(false)}
      />
    </div>
  );
};

export default SearchPage;