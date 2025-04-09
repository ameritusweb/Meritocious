import React, { useState } from 'react';
import { 
  Brain, Home, Search, GitFork, Star, Compass, 
  Users, BookOpen, Settings, Bell, TreeDeciduous,
  ChevronLeft, ChevronRight, Zap
} from 'lucide-react';

// Merit indicator component
const MeritIndicator = ({ score }) => (
  <div className="flex items-center gap-1 px-2 py-1 bg-teal-500/20 rounded text-teal-400 text-sm">
    <Star size={12} />
    <span>{score.toFixed(1)}</span>
  </div>
);

// Notification badge component
const NotificationBadge = ({ count }) => (
  <div className="px-1.5 py-0.5 bg-teal-500 rounded-full text-xs text-white min-w-[20px] text-center">
    {count}
  </div>
);

const Sidebar = () => {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [activeItem, setActiveItem] = useState('home');

  const mainNavItems = [
    { id: 'home', label: 'Home', icon: Home },
    { id: 'explore', label: 'Explore', icon: Compass },
    { id: 'search', label: 'Search', icon: Search },
    { id: 'forks', label: 'My Forks', icon: GitFork, merit: 4.2 },
    { id: 'tree', label: 'Idea Tree', icon: TreeDeciduous }
  ];

  const communityItems = [
    { id: 'trending', label: 'Trending', icon: Zap, notifications: 3 },
    { id: 'substacks', label: 'Substacks', icon: BookOpen },
    { id: 'people', label: 'People', icon: Users }
  ];

  const NavItem = ({ item, isCollapsed }) => (
    <button
      onClick={() => setActiveItem(item.id)}
      className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
        activeItem === item.id
          ? 'bg-teal-500/20 text-teal-400'
          : 'text-gray-400 hover:bg-gray-800/50 hover:text-gray-300'
      }`}
    >
      <item.icon size={20} />
      {!isCollapsed && (
        <div className="flex-1 flex items-center justify-between">
          <span>{item.label}</span>
          {item.merit && <MeritIndicator score={item.merit} />}
          {item.notifications && <NotificationBadge count={item.notifications} />}
        </div>
      )}
    </button>
  );

  return (
    <div 
      className={`h-screen bg-gray-900/95 backdrop-blur-xl border-r border-gray-800 
                  transition-all duration-300 flex flex-col ${
                    isCollapsed ? 'w-20' : 'w-64'
                  }`}
    >
      {/* Logo Section */}
      <div className="p-4 border-b border-gray-800">
        <div className="flex items-center gap-3">
          <div className="p-2 bg-teal-500/10 rounded-lg">
            <Brain className="w-6 h-6 text-teal-400" />
          </div>
          {!isCollapsed && (
            <span className="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-teal-400 to-teal-500">
              Meritocious
            </span>
          )}
        </div>
      </div>

      {/* Main Navigation */}
      <div className="flex-1 overflow-y-auto px-3 py-6 space-y-1">
        {/* Main items */}
        <div className="space-y-1 mb-8">
          {mainNavItems.map(item => (
            <NavItem key={item.id} item={item} isCollapsed={isCollapsed} />
          ))}
        </div>

        {/* Community Section */}
        {!isCollapsed && (
          <div className="text-xs font-medium text-gray-500 px-4 mb-2">
            COMMUNITY
          </div>
        )}
        <div className="space-y-1">
          {communityItems.map(item => (
            <NavItem key={item.id} item={item} isCollapsed={isCollapsed} />
          ))}
        </div>
      </div>

      {/* User Section */}
      <div className="p-4 border-t border-gray-800">
        <div className="flex items-center gap-3 mb-4">
          {!isCollapsed && (
            <>
              <div className="w-8 h-8 bg-gray-700 rounded-full" />
              <div className="flex-1">
                <div className="text-sm text-gray-300">AIResearcher</div>
                <div className="text-xs text-gray-500">View Profile</div>
              </div>
            </>
          )}
        </div>

        <div className="space-y-1">
          <NavItem 
            item={{ id: 'notifications', label: 'Notifications', icon: Bell, notifications: 5 }} 
            isCollapsed={isCollapsed} 
          />
          <NavItem 
            item={{ id: 'settings', label: 'Settings', icon: Settings }} 
            isCollapsed={isCollapsed} 
          />
        </div>
      </div>

      {/* Collapse Toggle */}
      <button
        onClick={() => setIsCollapsed(!isCollapsed)}
        className="absolute top-1/2 -right-3 w-6 h-6 bg-gray-800 rounded-full 
                   flex items-center justify-center text-gray-400 hover:text-gray-300"
      >
        {isCollapsed ? <ChevronRight size={14} /> : <ChevronLeft size={14} />}
      </button>
    </div>
  );
};

// Demo component
const Demo = () => (
  <div className="flex min-h-screen bg-gray-900">
    <Sidebar />
    <div className="flex-1 p-8">
      <h1 className="text-2xl text-white">Main Content Area</h1>
    </div>
  </div>
);

export default Demo;