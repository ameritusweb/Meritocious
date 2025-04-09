import React, { useState } from 'react';
import { GitFork, Share2, Flower2, GitBranch, Brain, 
         TreeDeciduous, Scale, FileSearch, Book } from 'lucide-react';

// Expandable section component
const FilterSection = ({ title, icon: Icon, children, defaultOpen = false }) => {
  const [isOpen, setIsOpen] = useState(defaultOpen);
  
  return (
    <div className="border border-gray-700/50 rounded-lg overflow-hidden">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="w-full px-4 py-3 bg-gray-800/50 flex items-center justify-between hover:bg-gray-700/50"
      >
        <div className="flex items-center gap-2 text-gray-300">
          <Icon size={18} className="text-teal-400" />
          <span className="font-medium">{title}</span>
        </div>
        <div className={`transform transition-transform ${isOpen ? 'rotate-180' : ''}`}>
          ▼
        </div>
      </button>
      
      {isOpen && (
        <div className="p-4 bg-gray-800/30">
          {children}
        </div>
      )}
    </div>
  );
};

// Fork type selection with categorization
const ForkTypeFilter = ({ selectedTypes, onToggleType }) => {
  const forkTypes = {
    'Analytical': [
      { id: 'extension', label: 'Extension Fork', icon: GitBranch },
      { id: 'critique', label: 'Critique Fork', icon: Scale },
      { id: 'synthesis', label: 'Synthesis Fork', icon: Brain },
      { id: 'application', label: 'Application Fork', icon: FileSearch }
    ],
    'Creative': [
      { id: 'what-if', label: 'What-If Fork', icon: Flower2 },
      { id: 'world', label: 'World Remix', icon: TreeDeciduous },
      { id: 'perspective', label: 'Perspective Fork', icon: Share2 },
      { id: 'narrative', label: 'Narrative Fork', icon: Book }
    ]
  };

  return (
    <div className="space-y-4">
      {Object.entries(forkTypes).map(([category, types]) => (
        <div key={category} className="space-y-2">
          <h4 className="text-sm text-gray-400">{category} Forks</h4>
          <div className="grid grid-cols-1 gap-2">
            {types.map(type => (
              <button
                key={type.id}
                onClick={() => onToggleType(type.id)}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-lg transition-colors ${
                  selectedTypes.includes(type.id)
                    ? 'bg-teal-500 text-white'
                    : 'bg-gray-700/50 text-gray-300 hover:bg-gray-600/50'
                }`}
              >
                <type.icon size={16} />
                <span className="text-sm">{type.label}</span>
              </button>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
};

// Lineage metrics filter
const LineageFilter = ({ values, onChange }) => {
  const metrics = [
    { id: 'generations', label: 'Generation Depth', min: 0, max: 10, step: 1 },
    { id: 'remixes', label: 'Remix Count', min: 0, max: 20, step: 1 },
    { id: 'bloom', label: 'Bloom Score', min: 0, max: 16, step: 1 }
  ];

  return (
    <div className="space-y-4">
      {metrics.map(metric => (
        <div key={metric.id} className="space-y-2">
          <div className="flex justify-between text-sm">
            <span className="text-gray-300">{metric.label}</span>
            <span className="text-teal-400">≥ {values[metric.id]}</span>
          </div>
          <input
            type="range"
            min={metric.min}
            max={metric.max}
            step={metric.step}
            value={values[metric.id]}
            onChange={(e) => onChange(metric.id, parseInt(e.target.value))}
            className="w-full accent-teal-500 bg-gray-700"
          />
          <div className="flex justify-between text-xs text-gray-500">
            <span>{metric.min}</span>
            <span>{metric.max}</span>
          </div>
        </div>
      ))}
    </div>
  );
};

// Main filter panel component
const AdvancedFilterPanel = ({ isOpen, onClose }) => {
  const [selectedForkTypes, setSelectedForkTypes] = useState(['extension', 'critique']);
  const [lineageMetrics, setLineageMetrics] = useState({
    generations: 2,
    remixes: 5,
    bloom: 8
  });

  const handleMetricChange = (metricId, value) => {
    setLineageMetrics(prev => ({
      ...prev,
      [metricId]: value
    }));
  };

  const toggleForkType = (typeId) => {
    setSelectedForkTypes(prev => 
      prev.includes(typeId)
        ? prev.filter(id => id !== typeId)
        : [...prev, typeId]
    );
  };

  return (
    <div className={`fixed inset-y-0 right-0 w-96 bg-gray-800/95 backdrop-blur-xl shadow-2xl 
                    transform transition-transform duration-300 ease-in-out z-50
                    ${isOpen ? 'translate-x-0' : 'translate-x-full'}`}>
      <div className="h-full flex flex-col">
        <div className="p-6 space-y-6 flex-grow overflow-y-auto">
          <h2 className="text-xl font-bold text-white mb-6 flex items-center gap-2">
            <GitFork className="text-teal-400" />
            Evolution Filters
          </h2>

          <div className="space-y-4">
            {/* Fork Types */}
            <FilterSection title="Fork Types" icon={GitBranch} defaultOpen={true}>
              <ForkTypeFilter
                selectedTypes={selectedForkTypes}
                onToggleType={toggleForkType}
              />
            </FilterSection>

            {/* Lineage Metrics */}
            <FilterSection title="Lineage Depth" icon={TreeDeciduous} defaultOpen={true}>
              <LineageFilter
                values={lineageMetrics}
                onChange={handleMetricChange}
              />
            </FilterSection>

            {/* Quick Presets */}
            <div className="pt-4">
              <h3 className="text-sm font-medium text-gray-300 mb-3">Evolution Patterns</h3>
              <div className="space-y-2">
                {[
                  { name: 'Deep Evolution', desc: '3+ generations, high bloom' },
                  { name: 'Active Remix', desc: '5+ remixes in past week' },
                  { name: 'Breakthrough Ideas', desc: 'High merit + multiple forks' }
                ].map(preset => (
                  <button
                    key={preset.name}
                    className="w-full px-4 py-3 bg-gray-700/50 rounded-lg text-left hover:bg-gray-600/50"
                  >
                    <div className="text-gray-300">{preset.name}</div>
                    <div className="text-sm text-gray-400">{preset.desc}</div>
                  </button>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* Action buttons */}
        <div className="p-6 border-t border-gray-700">
          <div className="flex gap-4">
            <button className="flex-1 px-4 py-3 bg-gray-700 rounded-lg text-gray-300 hover:bg-gray-600">
              Reset
            </button>
            <button 
              onClick={onClose}
              className="flex-1 px-4 py-3 bg-teal-500 rounded-lg text-white hover:bg-teal-400"
            >
              Apply Filters
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

// Demo component
const Demo = () => {
  const [isOpen, setIsOpen] = useState(true);
  return (
    <div className="min-h-screen bg-gray-900">
      <AdvancedFilterPanel isOpen={isOpen} onClose={() => setIsOpen(false)} />
    </div>
  );
};

export default Demo;