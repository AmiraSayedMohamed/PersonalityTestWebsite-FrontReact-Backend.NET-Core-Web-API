import React, { useState, useEffect } from 'react';
import './App.css';

const BACKEND_URL = 'http://localhost:5000/api';

function App() {
  const [currentStep, setCurrentStep] = useState('welcome'); // welcome, test, report
  const [session, setSession] = useState(null);
  const [currentQuestion, setCurrentQuestion] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [report, setReport] = useState(null);
  
  // New state for additional user info
  const [userName, setUserName] = useState('');
  const [gender, setGender] = useState('');
  const [birthYear, setBirthYear] = useState('');
  const [educationLevel, setEducationLevel] = useState('');
  const [maritalStatus, setMaritalStatus] = useState('');

  const [selectedAnswer, setSelectedAnswer] = useState(null);
  const [progress, setProgress] = useState(0);

  // Helper function to get dimension index for progress calculation
  const getCurrentDimensionIndex = (dimension) => {
    const dimensionOrder = ['openness', 'conscientiousness', 'extraversion', 'agreeableness', 'neuroticism'];
    const index = dimensionOrder.indexOf(dimension);
    return index >= 0 ? index : 0; // Return 0 if not found instead of -1
  };

  // Helper function to get Arabic name for dimension
  const getBigFiveName = (dimension) => {
    const dimensionNames = {
      'openness': 'الانفتاح على التجارب',
      'conscientiousness': 'الضمير الحي', 
      'extraversion': 'الانبساط',
      'agreeableness': 'المقبولية',
      'neuroticism': 'العصابية'
    };
    return dimensionNames[dimension] || dimension;
  };

  // Start new test session
  const startTest = async () => {
    if (!userName.trim()) {
      setError('الرجاء إدخال اسمك');
      return;
    }

    setLoading(true);
    setError('');
    
    console.log('Starting test with data:', {
      name: userName,
      gender: gender,
      birthYear: birthYear ? parseInt(birthYear) : null,
      educationLevel: educationLevel,
      maritalStatus: maritalStatus
    });

    try {
      console.log('Sending request to:', `${BACKEND_URL}/sessions`);
      const response = await fetch(`${BACKEND_URL}/sessions`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          name: userName,
          gender: gender,
          birthYear: birthYear ? parseInt(birthYear) : null,
          educationLevel: educationLevel,
          maritalStatus: maritalStatus
        })
      });

      console.log('Response status:', response.status);
      console.log('Response headers:', response.headers);
      
      if (!response.ok) {
        const errorText = await response.text();
        console.log('Error response text:', errorText);
        console.log('Response status text:', response.statusText);
        throw new Error(`فشل في إنشاء جلسة الاختبار: ${response.status} ${response.statusText}`);
      }

      const sessionData = await response.json();
      console.log('Session Data:', sessionData);  // للتحقق من البيانات
      setSession(sessionData);
      setCurrentStep('test');
      await loadCurrentQuestion(sessionData.sessionId);
    } catch (err) {
      console.error('Full error object:', err);
      setError(err.message || 'خطأ غير معروف في إنشاء الجلسة');
    } finally {
      setLoading(false);
    }
  };

  // Load current question
  const loadCurrentQuestion = async (sessionId) => {
    setLoading(true);
    console.log('Loading question for session:', sessionId); // للتحقق
    try {
      const response = await fetch(`${BACKEND_URL}/sessions/${sessionId}/question`);
      console.log('Response status:', response.status); // للتحقق
      
      if (!response.ok) {
        const errorText = await response.text();
        console.log('Error response:', errorText); // للتحقق
        throw new Error('فشل في تحميل السؤال');
      }

      const questionData = await response.json();
      console.log('Question Data:', questionData); // للتحقق من البيانات
      
      // Check if test is completed
      if (questionData.status === 'completed') {
        console.log('Test completed, loading report...');
        await loadReport();
        return;
      }
      
      setCurrentQuestion(questionData);
      setSelectedAnswer(null);
      
      // Calculate progress based on total questions answered across all dimensions
      // Get total questions answered so far
      const totalQuestionsAnswered = questionData && questionData.questionNumber && questionData.dimension ? 
        (questionData.questionNumber - 1) + (getCurrentDimensionIndex(questionData.dimension) * 10) : 
        0;
      setProgress((totalQuestionsAnswered / 50) * 100);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Submit answer
  const submitAnswer = async () => {
    if (selectedAnswer === null) {
      setError('الرجاء اختيار إجابة');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${BACKEND_URL}/answers`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          sessionId: session.sessionId,
          questionId: currentQuestion.questionId,
          response: selectedAnswer
        })
      });

      if (!response.ok) {
        throw new Error('فشل في حفظ الإجابة');
      }

      const result = await response.json();
      
      if (result.status === 'completed') {
        // Test completed, load report
        await loadReport();
      } else {
        // Continue to next question
        await loadCurrentQuestion(session.sessionId);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Load personality report
  const loadReport = async () => {
    setLoading(true);
    try {
      console.log('Loading report for session:', session.sessionId);
      const response = await fetch(`${BACKEND_URL}/sessions/${session.sessionId}/report`);
      
      console.log('Report response status:', response.status);
      if (!response.ok) {
        const errorText = await response.text();
        console.log('Report error response:', errorText);
        throw new Error('فشل في تحميل التقرير');
      }

      const reportData = await response.json();
      console.log('Report data received:', reportData);
      setReport(reportData);
      setCurrentStep('report');
    } catch (err) {
      console.error('Error loading report:', err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Reset test
  const resetTest = () => {
    setCurrentStep('welcome');
    setSession(null);
    setCurrentQuestion(null);
    setReport(null);
    setUserName('');
    setGender('');
    setBirthYear('');
    setEducationLevel('');
    setMaritalStatus('');
    setSelectedAnswer(null);
    setProgress(0);
    setError('');
  };

  // Answer scale labels in Arabic
  const answerLabels = [
    'غير صحيح تماماً',
    'غير صحيح نوعاً ما',
    'محايد',
    'صحيح نوعاً ما',
    'صحيح تماماً'
  ];

  return (
    <div className="app" dir="rtl">
      <div className="container">
        {/* Header */}
        <header className="header">
          <h1 className="title">اختبار الشخصية التكيفي</h1>
          <p className="subtitle">اكتشف شخصيتك من خلال نموذج الشخصية الخماسي</p>
        </header>

        {/* Error Message */}
        {error && (
          <div className="error-message">
            <div className="error-content">
              <span className="error-icon">⚠️</span>
              <span>{error}</span>
            </div>
          </div>
        )}

        {/* Welcome Step */}
        {currentStep === 'welcome' && (
          <div className="step-container">
            <div className="welcome-card">
              <div className="welcome-icon">🧠</div>
              <h2 className="welcome-title">مرحباً بك في اختبار الشخصية</h2>
              <p className="welcome-description">
                سيساعدك هذا الاختبار على فهم شخصيتك بناءً على نموذج الشخصية الخماسي المعترف به علمياً.
                سيستغرق الاختبار حوالي 10-15 دقيقة ويتكون من 30-40 سؤال.
              </p>
              
              <div className="form-group">
                <label className="form-label">الاسم *</label>
                <input
                  type="text"
                  className="form-input"
                  placeholder="أدخل اسمك الكامل"
                  value={userName}
                  onChange={(e) => setUserName(e.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label className="form-label">سنة الميلاد</label>
                <input
                  type="number"
                  className="form-input"
                  placeholder="أدخل سنة ميلادك"
                  value={birthYear}
                  onChange={(e) => setBirthYear(e.target.value)}
                />
              </div>

              <div className="form-group">
                <label className="form-label">الجنس</label>
                <select 
                  className="form-input" 
                  value={gender} 
                  onChange={(e) => setGender(e.target.value)}
                >
                  <option value="">اختر الجنس</option>
                  <option value="male">ذكر</option>
                  <option value="female">أنثى</option>
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">المستوى التعليمي</label>
                <select 
                  className="form-input" 
                  value={educationLevel} 
                  onChange={(e) => setEducationLevel(e.target.value)}
                >
                  <option value="">اختر المستوى التعليمي</option>
                  <option value="high_school">ثانوية عامة أو أقل</option>
                  <option value="diploma">دبلوم</option>
                  <option value="bachelor">بكالوريوس</option>
                  <option value="master">ماجستير</option>
                  <option value="phd">دكتوراه</option>
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">الحالة الاجتماعية</label>
                <select 
                  className="form-input" 
                  value={maritalStatus} 
                  onChange={(e) => setMaritalStatus(e.target.value)}
                >
                  <option value="">اختر الحالة الاجتماعية</option>
                  <option value="single">أعزب/عزباء</option>
                  <option value="married">متزوج/متزوجة</option>
                  <option value="divorced">مطلق/مطلقة</option>
                  <option value="widowed">أرمل/أرملة</option>
                </select>
              </div>

              <button
                className="primary-button"
                onClick={startTest}
                disabled={loading}
              >
                {loading ? (
                  <span className="loading-spinner">جاري التحميل...</span>
                ) : (
                  'ابدأ الاختبار'
                )}
              </button>

              <div className="info-cards">
                <div className="info-card">
                  <h3>🎯 دقيق علمياً</h3>
                  <p>مبني على نموذج الشخصية الخماسي المعترف به عالمياً</p>
                </div>
                <div className="info-card">
                  <h3>🤖 تكيفي بالذكاء الاصطناعي</h3>
                  <p>أسئلة مولدة بالذكاء الاصطناعي تتكيف مع إجاباتك</p>
                </div>
                <div className="info-card">
                  <h3>📊 تقرير شامل</h3>
                  <p>تحليل مفصل لشخصيتك مع توصيات للتطوير</p>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Test Step */}
        {currentStep === 'test' && currentQuestion && (
          <div className="step-container">
            <div className="test-card">
              {/* Progress Bar */}
              <div className="progress-container">
                <div className="progress-info">
                  <span>السؤال {currentQuestion && currentQuestion.dimension && currentQuestion.questionNumber ? 
                    (getCurrentDimensionIndex(currentQuestion.dimension) * 10) + currentQuestion.questionNumber : 
                    1
                  } من 50</span>
                  <span>{Math.round(progress)}%</span>
                </div>
                <div className="progress-bar">
                  <div 
                    className="progress-fill" 
                    style={{ width: `${progress}%` }}
                  ></div>
                </div>
              </div>

              {/* Question */}
              <div className="question-container">
                <h2 className="question-text">{currentQuestion.text}</h2>
              </div>

              {/* Answer Options */}
              <div className="answers-container">
                <p className="answers-instruction">اختر الإجابة التي تصف شخصيتك بشكل أفضل:</p>
                <div className="answer-options">
                  {answerLabels.map((label, index) => (
                    <button
                      key={index + 1}
                      className={`answer-option ${selectedAnswer === index + 1 ? 'selected' : ''}`}
                      onClick={() => setSelectedAnswer(index + 1)}
                    >
                      <div className="answer-number">{index + 1}</div>
                      <div className="answer-label">{label}</div>
                    </button>
                  ))}
                </div>
              </div>

              {/* Navigation */}
              <div className="navigation-container">
                <button
                  className="primary-button"
                  onClick={submitAnswer}
                  disabled={loading || selectedAnswer === null}
                >
                  {loading ? (
                    <span className="loading-spinner">جاري الحفظ...</span>
                  ) : (
                    'السؤال التالي'
                  )}
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Report Step */}
        {currentStep === 'report' && report && (
          <div className="step-container">
            <div className="report-card">
              <div className="report-header">
                <h2 className="report-title">تقرير شخصيتك - {report.name}</h2>
                <p className="report-date">تاريخ الإكمال: {new Date(report.completionDate).toLocaleDateString('ar-SA')}</p>
              </div>

              {/* Scores */}
              <div className="scores-section">
                <h3 className="section-title">نتائج أبعاد الشخصية الخمسة</h3>
                <div className="scores-grid">
                  {Object.entries(report.scores).map(([dimension, data]) => (
                    <div key={dimension} className="score-card">
                      <h4 className="score-dimension">{data.name}</h4>
                      <div className="score-visual">
                        <div className="score-bar">
                          <div 
                            className="score-fill"
                            style={{ width: `${(data.score / 5) * 100}%` }}
                          ></div>
                        </div>
                        <div className="score-info">
                          <span className="score-value">{data.score.toFixed(1)}/5</span>
                          <span className={`score-level ${data.level.toLowerCase()}`}>{data.level}</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Detailed Analysis */}
              <div className="analysis-section">
                <h3 className="section-title">التحليل المفصل</h3>
                <div className="analysis-content">
                  {report.detailedAnalysis ? report.detailedAnalysis.split('\n').map((paragraph, index) => (
                    paragraph.trim() && <p key={index} className="analysis-paragraph">{paragraph}</p>
                  )) : <p>لا يوجد تحليل مفصل متاح</p>}
                </div>
              </div>

              {/* Recommendations */}
              <div className="recommendations-section">
                <h3 className="section-title">توصيات للتطوير</h3>
                <div className="recommendations-list">
                  {report.recommendations && report.recommendations.length > 0 ? report.recommendations.map((recommendation, index) => (
                    <div key={index} className="recommendation-item">
                      <span className="recommendation-icon">💡</span>
                      <span className="recommendation-text">{recommendation}</span>
                    </div>
                  )) : <p>لا توجد توصيات متاحة</p>}
                </div>
              </div>

              {/* Actions */}
              <div className="report-actions">
                <button className="secondary-button" onClick={resetTest}>
                  إجراء اختبار جديد
                </button>
                <button 
                  className="primary-button"
                  onClick={() => window.print()}
                >
                  طباعة التقرير
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Loading Overlay */}
        {loading && currentStep !== 'welcome' && (
          <div className="loading-overlay">
            <div className="loading-content">
              <div className="loading-spinner-large"></div>
              <p>جاري المعالجة...</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;