using NUnit.Framework;
using UnityEngine;

namespace Besttof.SelectionHistory
{
	[TestFixture]
	public class SelectionHistoryTests
	{
		private GameObject _gameObject3;
		private GameObject _gameObject2;
		private GameObject _gameObject1;

		[SetUp]
		public void SetUp()
		{
			_gameObject1 = new GameObject();
			_gameObject2 = new GameObject();
			_gameObject3 = new GameObject();
			
		}
		
		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(_gameObject1);
			Object.DestroyImmediate(_gameObject2);
			Object.DestroyImmediate(_gameObject3);
		}

		[Test]
		public void SequenceEquals_IsCorrect()
		{
			var sequence1 = new Object[] { _gameObject1, _gameObject2, _gameObject3};
			var sequence2 = new Object[] { _gameObject1, _gameObject2, _gameObject3};
			
			var result = SelectionHistoryManager.SequenceEquals(sequence1, sequence2);
			
			Assert.That(result, Is.True);
		}
		
		[Test]
		public void SequenceEquals_WhenAllNull_IsCorrect()
		{
			var sequence1 = new Object[] { null, null, null};
			var sequence2 = new Object[] { null, null, null};
			
			var result = SelectionHistoryManager.SequenceEquals(sequence1, sequence2);
			
			Assert.That(result, Is.True);
		}
		
		[Test]
		public void SequenceEquals_WhenSomeNull_IsCorrect()
		{
			var sequence1 = new Object[] { null, _gameObject1, null};
			var sequence2 = new Object[] { null, _gameObject1, null};
			
			var result = SelectionHistoryManager.SequenceEquals(sequence1, sequence2);
			
			Assert.That(result, Is.True);
		}
		
		[Test]
		public void SequenceEquals_WithDifferentSequences_IsCorrect()
		{
			var sequence1 = new Object[] { _gameObject3, null, _gameObject2};
			var sequence2 = new Object[] { _gameObject1, null, _gameObject3};
			
			var result = SelectionHistoryManager.SequenceEquals(sequence1, sequence2);
			
			Assert.That(result, Is.False);
		}
	}
}