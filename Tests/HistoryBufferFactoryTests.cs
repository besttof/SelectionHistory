using NUnit.Framework;

namespace Besttof.SelectionHistory
{
	[TestFixture]
	public class HistoryBufferFactoryTests
	{
		[Test]
		public void Push_IncreasesCount()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(3);
			buffer.Push(1);
			buffer.Push(3);
			buffer.Push(5);

			Assert.That(buffer.Count, Is.EqualTo(3));
			Assert.That(buffer.TryGetCurrent(out var current), Is.True);
			Assert.That(current, Is.EqualTo(5));
		}

		[Test]
		public void Push_CountStaysAtCapacity_WhenPuhsingMoreThanCount()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(3);
			buffer.Push(1);
			buffer.Push(3);
			buffer.Push(5);
			buffer.Push(42);

			Assert.That(buffer.Count, Is.EqualTo(3));
			Assert.That(buffer.TryGetCurrent(out var current), Is.True);
			Assert.That(current, Is.EqualTo(42));
		}

		[Test]
		public void TryGetCurrent_ReturnsFalse_WhenCursorIsAtStart()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			var result = buffer.TryGetCurrent(out var value);

			Assert.That(result, Is.False);
		}

		[Test]
		public void TryGetCurrent_Succeeds_WhenItemsArePushed()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			buffer.Push(42);
			var result = buffer.TryGetCurrent(out var value);

			Assert.That(result, Is.True);
			Assert.That(value, Is.EqualTo(42));
		}

		[Test]
		public void TryGoBack_Fails_WhenCursorIsAtStart()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			var result = buffer.TryGoBack(out var value);

			Assert.That(result, Is.False);
		}

		[Test]
		public void TryGoBack_Succeeds_WhenCursorIsNotAtStart()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			buffer.Push(1);
			buffer.Push(5);
			buffer.Push(42);

			var result = buffer.TryGoBack(out var value);

			Assert.That(result, Is.True);
			Assert.That(value, Is.EqualTo(5));
		}
		
		[Test]
		public void TryGoForward_Fails_WhenCursorIsAtLastItem()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			buffer.Push(1);
			buffer.Push(5);
			buffer.Push(42);

			var result = buffer.TryGoForward(out var value);

			Assert.That(result, Is.False);
		}
		
		[Test]
		public void TryGoForward_Succeeds_WhenCursorIsNotAtLastItem()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			buffer.Push(1);
			buffer.Push(5);
			buffer.Push(42);

			var backResult = buffer.TryGoBack(out var backValue);
			var result = buffer.TryGoForward(out var value);

			Assert.That(backResult, Is.True);
			Assert.That(result, Is.True);
			Assert.That(value, Is.EqualTo(42));
		}

		[Test]
		public void Push_TruncatesCount_WhenCursorIsNotAtEnd()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			buffer.Push(1);
			buffer.Push(3);
			buffer.Push(5);

			var countBefore = buffer.Count;
			
			var backResult1 = buffer.TryGoBack(out _);
			var backResult2 = buffer.TryGoBack(out _);
			
			buffer.Push(42);
			
			var count = buffer.Count;

			Assert.That(backResult1, Is.True);
			Assert.That(backResult2, Is.True);
			Assert.That(countBefore, Is.EqualTo(3));
			Assert.That(count, Is.EqualTo(2));
		}

		[Test]
		public void Clear_RemovesAllItems()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);

			buffer.Push(1);
			buffer.Push(3);
			buffer.Push(5);
			buffer.Clear();
			
			Assert.That(buffer.Count, Is.EqualTo(0));
			Assert.That(buffer.TryGetCurrent(out _), Is.False);
		}
		
		[Test]
		public void Clear_HasNoEffect_WhenBufferIsEmpty()
		{
			var buffer = HistoryBufferFactory.CreateRaw<int>(5);
			buffer.Clear();
			
			Assert.That(buffer.Count, Is.EqualTo(0));
			Assert.That(buffer.TryGetCurrent(out _), Is.False);
		}
	}
}