import React, { useState } from 'react';
import { Heart, GitFork, MessageSquare, ArrowRight, Star, ThumbsUp, Reply } from 'lucide-react';

// Mini merit indicator for comments
const CommentMerit = ({ score }) => (
  <div className="flex items-center gap-1 px-2 py-1 bg-gray-800/50 rounded text-sm">
    <Heart className={`w-4 h-4 ${score >= 4 ? 'text-teal-400' : 'text-gray-500'}`} />
    <span className={score >= 4 ? 'text-teal-400' : 'text-gray-400'}>{score.toFixed(1)}</span>
  </div>
);

// Fork suggestion banner
const ForkSuggestion = ({ commentId }) => (
  <div className="mt-4 p-4 bg-teal-500/10 rounded-lg border border-teal-500/20">
    <div className="flex items-center justify-between">
      <div className="flex items-center gap-3">
        <GitFork className="w-5 h-5 text-teal-400" />
        <span className="text-sm text-teal-300">This comment shows potential for a new perspective</span>
      </div>
      <button className="px-3 py-1.5 bg-teal-500/20 rounded-lg text-sm text-teal-400 hover:bg-teal-500/30 flex items-center gap-2">
        Fork This <ArrowRight size={16} />
      </button>
    </div>
  </div>
);

// Comment composer
const CommentComposer = ({ onSubmit, placeholder = "Add to the discussion..." }) => {
  const [content, setContent] = useState('');
  
  return (
    <div className="space-y-4">
      <textarea
        value={content}
        onChange={(e) => setContent(e.target.value)}
        placeholder={placeholder}
        className="w-full h-32 px-4 py-3 bg-gray-800/50 rounded-lg border border-gray-700/50
                 text-white placeholder-gray-500 focus:outline-none focus:border-teal-500/50
                 focus:ring-1 focus:ring-teal-500/50 resize-none"
      />
      <div className="flex justify-end">
        <button 
          onClick={() => {
            onSubmit(content);
            setContent('');
          }}
          disabled={!content.trim()}
          className="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-400 
                   disabled:bg-gray-700 disabled:text-gray-400 disabled:cursor-not-allowed"
        >
          Post Comment
        </button>
      </div>
    </div>
  );
};

// Comment thread component
const Comment = ({ comment, depth = 0 }) => {
  const [showReply, setShowReply] = useState(false);
  
  return (
    <div className={`${depth > 0 ? 'ml-8 pl-8 border-l border-gray-700/50' : ''}`}>
      <div className="p-6 bg-gray-800/30 rounded-lg">
        {/* Comment header */}
        <div className="flex items-start justify-between mb-4">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 bg-gray-700 rounded-full" />
            <div>
              <div className="text-white font-medium">{comment.author}</div>
              <div className="text-sm text-gray-400">{comment.timestamp}</div>
            </div>
          </div>
          <CommentMerit score={comment.merit} />
        </div>

        {/* Comment content */}
        <div className="text-gray-300 mb-4">{comment.content}</div>

        {/* Comment actions */}
        <div className="flex items-center gap-4 text-sm">
          <button className="text-gray-400 hover:text-teal-400 flex items-center gap-1">
            <ThumbsUp size={16} />
            <span>{comment.likes}</span>
          </button>
          <button 
            onClick={() => setShowReply(!showReply)}
            className="text-gray-400 hover:text-teal-400 flex items-center gap-1"
          >
            <Reply size={16} />
            <span>Reply</span>
          </button>
          <button className="text-gray-400 hover:text-teal-400 flex items-center gap-1">
            <GitFork size={16} />
            <span>Fork Thread</span>
          </button>
        </div>

        {/* Fork suggestion if merit is high */}
        {comment.merit >= 4.5 && <ForkSuggestion commentId={comment.id} />}

        {/* Reply composer */}
        {showReply && (
          <div className="mt-4">
            <CommentComposer 
              onSubmit={(content) => console.log('Reply:', content)} 
              placeholder="Write a reply..."
            />
          </div>
        )}
      </div>

      {/* Nested replies */}
      {comment.replies && (
        <div className="mt-4 space-y-4">
          {comment.replies.map((reply) => (
            <Comment key={reply.id} comment={reply} depth={depth + 1} />
          ))}
        </div>
      )}
    </div>
  );
};

// Discussion section
const DiscussionThread = () => {
  const mockComments = [
    {
      id: 1,
      author: "GameTheoryExpert",
      timestamp: "1 day ago",
      content: "This is a fascinating perspective on recursive self-improvement. The parallel with evolutionary stable strategies is particularly insightful. Have you considered how this might apply to multi-agent systems where each agent has different initial utility functions?",
      merit: 4.7,
      likes: 24,
      replies: [
        {
          id: 2,
          author: "AIResearcher",
          timestamp: "1 day ago",
          content: "That's a great point about multi-agent systems! The stability characteristics would indeed be different when we have multiple agents with varying utility functions. We might be able to use concepts from coalition game theory here.",
          merit: 4.5,
          likes: 18,
          replies: [
            {
              id: 3,
              author: "SystemsEngineer",
              timestamp: "12 hours ago",
              content: "The coalition game theory angle is promising. We could potentially model this as a cooperative game where agents form coalitions based on aligned utility functions.",
              merit: 4.2,
              likes: 12
            }
          ]
        }
      ]
    },
    {
      id: 4,
      author: "AlignmentResearcher",
      timestamp: "1 day ago",
      content: "While the evolutionary approach is interesting, we need to be careful about assuming natural selection principles will lead to aligned behavior. There might be stable states that are nevertheless undesirable from a human values perspective.",
      merit: 4.8,
      likes: 31
    }
  ];

  return (
    <div className="min-h-screen bg-gray-900 text-white">
      <div className="max-w-4xl mx-auto p-8">
        <div className="space-y-8">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-bold text-white">Discussion</h2>
            <div className="flex items-center gap-2">
              <span className="text-gray-400">Sort by:</span>
              <select className="px-3 py-1.5 bg-gray-800/50 rounded-lg text-gray-300 border border-gray-700/50">
                <option>Top Merit</option>
                <option>Newest</option>
                <option>Most Discussed</option>
              </select>
            </div>
          </div>

          <CommentComposer onSubmit={(content) => console.log('New comment:', content)} />

          <div className="space-y-4">
            {mockComments.map((comment) => (
              <Comment key={comment.id} comment={comment} />
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default DiscussionThread;