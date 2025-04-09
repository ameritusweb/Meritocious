import React, { useState } from 'react';
import { Heart, GitFork, MessageSquare, Star, Settings, ExternalLink, Filter, Clock, TrendingUp } from 'lucide-react';

// Merit bar component
const MeritBar = ({ score }) => (
  <div className="flex items-center gap-1">
    {[...Array(5)].map((_, i) => (
      <Heart key={i} 
            className={`w-5 h-5 ${i < score ? 'text-teal-400 fill-current' : 'text-gray-600'}`} />
    ))}
  </div>
);

// Activity card component
const ActivityCard = ({ title, type, merit, timestamp, content, forks, replies }) => (
  <div className="bg-gray-800/50 backdrop-blur-sm rounded-lg p-6 border border-gray-700/50">
    <div className="flex items-start justify-between mb-4">
      <div>
        <h3 className="text-lg font-medium text-white mb-1">{title}</h3>
        <div className="flex items-center gap-4 text-sm text-gray-400">
          <span className="px-2 py-1 bg-gray-700/50 rounded text-teal-400">{type}</span>
          <span className="flex items-center gap-1">
            <Clock size={14} />
            {timestamp}
          </span>
        </div>
      </div>
      <MeritBar score={merit} />
    </div>
    
    <p className="text-gray-300 mb-4">{content}</p>
    
    <div className="flex items-center gap-6 text-gray-400">
      <div className="flex items-center gap-2">
        <GitFork size={16} />
        <span>{forks} Forks</span>
      </div>
      <div className="flex items-center gap-2">
        <MessageSquare size={16} />
        <span>{replies} Replies</span>
      </div>
    </div>
  </div>
);

// Stats card component
const StatsCard = ({ icon: Icon, label, value, trend }) => (
  <div className="bg-gray-800/50 backdrop-blur-sm rounded-lg p-4 border border-gray-700/50">
    <div className="flex items-start justify-between">
      <div className="p-2 bg-gray-700/50 rounded">
        <Icon className="w-5 h-5 text-teal-400" />
      </div>
      {trend && (
        <div className="flex items-center gap-1 text-green-400 text-sm">
          <TrendingUp size={14} />
          {trend}
        </div>
      )}
    </div>
    <div className="mt-4">
      <div className="text-2xl font-bold text-white">{value}</div>
      <div className="text-sm text-gray-400">{label}</div>
    </div>
  </div>
);

const ProfilePage = () => {
  const [activeTab, setActiveTab] = useState('contributions');
  
  const userData = {
    username: "AIResearcher",
    bio: "Exploring the intersection of artificial intelligence and human knowledge systems.",
    interests: ["AI", "Philosophy", "Systems Theory", "Ethics"],
    joinDate: "Member since March 2025",
    totalMerit: 4.2
  };

  const stats = [
    { icon: Star, label: "Total Merit", value: "4.2", trend: "+0.3" },
    { icon: GitFork, label: "Forks Created", value: "23" },
    { icon: MessageSquare, label: "Discussions", value: "156" },
    { icon: Heart, label: "Ideas Evolved", value: "45" }
  ];

  const activities = [
    {
      title: "Recursive Self-Improvement: A New Framework",
      type: "Original Post",
      merit: 4.5,
      timestamp: "2 days ago",
      content: "Proposing a novel approach to recursive self-improvement in AI systems based on game theory and evolutionary stable strategies.",
      forks: 12,
      replies: 24
    },
    {
      title: "Re: Ethics of Digital Consciousness",
      type: "Fork",
      merit: 4.0,
      timestamp: "1 week ago",
      content: "Building on @PhilosopherAI's framework with insights from recent developments in neuromorphic computing.",
      forks: 8,
      replies: 15
    }
  ];

  return (
    <div className="min-h-screen bg-gray-900 text-white p-6">
      <div className="max-w-6xl mx-auto">
        {/* Profile Header */}
        <div className="bg-gray-800/50 backdrop-blur-sm rounded-lg p-8 border border-gray-700/50 mb-8">
          <div className="flex items-start justify-between mb-6">
            <div>
              <h1 className="text-3xl font-bold text-white mb-2">{userData.username}</h1>
              <p className="text-gray-400 mb-4">{userData.joinDate}</p>
              <p className="text-gray-300 max-w-2xl mb-4">{userData.bio}</p>
              <div className="flex flex-wrap gap-2">
                {userData.interests.map(interest => (
                  <span key={interest} className="px-3 py-1 bg-gray-700/50 rounded-full text-sm text-teal-400">
                    {interest}
                  </span>
                ))}
              </div>
            </div>
            <div className="flex items-center gap-4">
              <button className="px-4 py-2 bg-gray-700/50 rounded-lg text-gray-300 hover:bg-gray-600/50 flex items-center gap-2">
                <Settings size={18} />
                Edit Profile
              </button>
              <button className="px-4 py-2 bg-teal-500 rounded-lg text-white hover:bg-teal-400 flex items-center gap-2">
                <ExternalLink size={18} />
                Share Profile
              </button>
            </div>
          </div>

          {/* Stats Grid */}
          <div className="grid grid-cols-4 gap-4">
            {stats.map((stat, i) => (
              <StatsCard key={i} {...stat} />
            ))}
          </div>
        </div>

        {/* Content Tabs */}
        <div className="mb-6">
          <div className="flex items-center gap-4 border-b border-gray-700">
            {['Contributions', 'Forks', 'Discussions', 'Merit Log'].map(tab => (
              <button
                key={tab}
                onClick={() => setActiveTab(tab.toLowerCase())}
                className={`px-4 py-3 text-sm font-medium transition-colors ${
                  activeTab === tab.toLowerCase()
                    ? 'text-teal-400 border-b-2 border-teal-400'
                    : 'text-gray-400 hover:text-gray-300'
                }`}
              >
                {tab}
              </button>
            ))}
          </div>
        </div>

        {/* Filter Bar */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-2">
            <button className="px-4 py-2 bg-gray-800/50 rounded-lg text-gray-300 hover:bg-gray-700/50 flex items-center gap-2">
              <Filter size={16} />
              Filter
            </button>
            <select className="px-4 py-2 bg-gray-800/50 rounded-lg text-gray-300 border border-gray-700/50">
              <option>All Time</option>
              <option>This Month</option>
              <option>This Week</option>
            </select>
          </div>
          <div className="flex items-center gap-2">
            <span className="text-gray-400">Sort by:</span>
            <select className="px-4 py-2 bg-gray-800/50 rounded-lg text-gray-300 border border-gray-700/50">
              <option>Highest Merit</option>
              <option>Most Recent</option>
              <option>Most Forks</option>
            </select>
          </div>
        </div>

        {/* Activity Feed */}
        <div className="space-y-4">
          {activities.map((activity, i) => (
            <ActivityCard key={i} {...activity} />
          ))}
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;