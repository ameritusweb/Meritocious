import React, { useState } from 'react';
import { Brain, ArrowLeft, Mail, Lock, User, AtSign, Eye, EyeOff } from 'lucide-react';

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

const InputField = ({ icon: Icon, type, placeholder, value, onChange }) => {
  const [showPassword, setShowPassword] = useState(false);
  const isPassword = type === 'password';
  
  return (
    <div className="relative">
      <div className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">
        <Icon size={18} />
      </div>
      <input
        type={isPassword && showPassword ? 'text' : type}
        placeholder={placeholder}
        value={value}
        onChange={onChange}
        className="w-full px-10 py-3 bg-gray-800/50 border border-gray-700/50 rounded-lg
                 text-white placeholder-gray-400 focus:outline-none focus:border-teal-500/50
                 focus:ring-1 focus:ring-teal-500/50 backdrop-blur-sm"
      />
      {isPassword && (
        <button
          type="button"
          onClick={() => setShowPassword(!showPassword)}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-300"
        >
          {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
        </button>
      )}
    </div>
  );
};

const SignupPage = ({ onBack }) => {
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    interests: []
  });

  const interests = [
    'Technology', 'Philosophy', 'Science', 'Arts',
    'Politics', 'Economics', 'Environment', 'Education'
  ];

  // Generate positions for petals
  const petals = [...Array(15)].map(() => ({
    left: `${Math.random() * 100}%`,
    top: `${Math.random() * 100}%`,
    animationDelay: `${Math.random() * 5}s`,
    animationDuration: `${10 + Math.random() * 10}s`,
    opacity: 0.1 + Math.random() * 0.1,
    transform: `rotate(${Math.random() * 360}deg)`,
  }));

  const handleInterestToggle = (interest) => {
    setFormData(prev => ({
      ...prev,
      interests: prev.interests.includes(interest)
        ? prev.interests.filter(i => i !== interest)
        : [...prev.interests, interest]
    }));
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white relative overflow-hidden">
      {/* Background petals */}
      <div className="fixed inset-0">
        {petals.map((style, i) => (
          <FloatingPetal key={i} style={style} />
        ))}
      </div>

      {/* Content */}
      <div className="relative min-h-screen flex flex-col items-center justify-center p-6">
        {/* Back button */}
        <button
          onClick={onBack}
          className="absolute top-4 left-4 px-4 py-2 text-gray-300 hover:text-white
                   flex items-center gap-2 transition-colors"
        >
          <ArrowLeft size={20} />
          Back
        </button>

        {/* Main content */}
        <div className="w-full max-w-md">
          {/* Header */}
          <div className="text-center mb-8">
            <Brain className="w-12 h-12 text-teal-400 mx-auto mb-4" />
            <h1 className="text-2xl font-bold mb-2">Join Meritocious</h1>
            <p className="text-gray-400">Start your journey of idea evolution</p>
          </div>

          {/* Form */}
          <form className="space-y-6" onSubmit={(e) => e.preventDefault()}>
            <InputField
              icon={User}
              type="text"
              placeholder="Username"
              value={formData.username}
              onChange={(e) => setFormData(prev => ({ ...prev, username: e.target.value }))}
            />
            
            <InputField
              icon={Mail}
              type="email"
              placeholder="Email address"
              value={formData.email}
              onChange={(e) => setFormData(prev => ({ ...prev, email: e.target.value }))}
            />
            
            <InputField
              icon={Lock}
              type="password"
              placeholder="Password"
              value={formData.password}
              onChange={(e) => setFormData(prev => ({ ...prev, password: e.target.value }))}
            />

            {/* Interests */}
            <div className="space-y-3">
              <label className="text-sm text-gray-300">Select your interests</label>
              <div className="grid grid-cols-2 gap-2">
                {interests.map(interest => (
                  <button
                    key={interest}
                    type="button"
                    onClick={() => handleInterestToggle(interest)}
                    className={`px-4 py-2 rounded-lg text-sm transition-colors ${
                      formData.interests.includes(interest)
                        ? 'bg-teal-500 text-white'
                        : 'bg-gray-800/50 text-gray-300 hover:bg-gray-700/50'
                    }`}
                  >
                    {interest}
                  </button>
                ))}
              </div>
            </div>

            {/* Submit button */}
            <button
              type="submit"
              className="w-full px-6 py-3 bg-gradient-to-r from-teal-500 to-teal-400
                       text-white rounded-lg font-medium hover:from-teal-400 hover:to-teal-300
                       transition-all duration-300 transform hover:scale-[1.02]
                       hover:shadow-xl hover:shadow-teal-500/25"
            >
              Create Account
            </button>

            {/* Terms */}
            <p className="text-sm text-gray-400 text-center">
              By signing up, you agree to our{' '}
              <a href="#" className="text-teal-400 hover:underline">Terms of Service</a>
              {' '}and{' '}
              <a href="#" className="text-teal-400 hover:underline">Privacy Policy</a>
            </p>
          </form>

          {/* Login link */}
          <p className="mt-8 text-center text-gray-400">
            Already have an account?{' '}
            <a href="#" className="text-teal-400 hover:underline">Log in</a>
          </p>
        </div>
      </div>
    </div>
  );
};

export default SignupPage;