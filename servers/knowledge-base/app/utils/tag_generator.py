"""
Модуль для автоматического генерирования тегов из текста.
Используется простой подход на основе частоты слов, без тяжелых библиотек NLP.
"""

import re
import string
from collections import Counter
from typing import List, Set


STOP_WORDS = {
    'и', 'в', 'на', 'с', 'по', 'к', 'а', 'от', 'что', 'как', 'это', 'для', 'при',
    'или', 'также', 'не', 'так', 'но', 'то', 'да', 'бы', 'же', 'за', 'из', 'у',
    'без', 'до', 'во', 'со', 'под', 'о', 'об', 'про', 'через', 'над', 'только',
    'уже', 'еще', 'всё', 'все', 'ещё', 'вы', 'он', 'она', 'оно', 'они', 'мы', 'я',
    'этот', 'эта', 'это', 'эти', 'тот', 'та', 'те', 'который', 'которая', 'которое',
    'которые', 'кто', 'что', 'чей', 'где', 'когда', 'куда', 'откуда',
    'the', 'a', 'an', 'is', 'are', 'was', 'were', 'be', 'have', 'has', 'had',
    'do', 'does', 'did', 'will', 'would', 'can', 'could', 'should', 'may',
    'might', 'must', 'shall', 'and', 'or', 'but', 'if', 'while', 'of', 'at',
    'by', 'for', 'with', 'about', 'against', 'between', 'into', 'through',
    'during', 'before', 'after', 'above', 'below', 'to', 'from', 'up', 'down',
    'in', 'out', 'on', 'off', 'over', 'under', 'again', 'further', 'then',
    'once', 'here', 'there', 'when', 'where', 'why', 'how', 'all', 'any',
    'both', 'each', 'few', 'more', 'most', 'other', 'some', 'such', 'no',
    'nor', 'not', 'only', 'own', 'same', 'so', 'than', 'too', 'very',
    'this', 'that', 'these', 'those', 'which', 'whose', 'whom',
}


def clean_text(text: str) -> str:
    """
    Очищает текст от кода, знаков пунктуации, URL и лишних пробелов.

    Args:
        text: Исходный текст

    Returns:
        Очищенный текст
    """
    if not text:
        return ""


    text = re.sub(r'https?://\S+|www\.\S+', '', text)


    text = re.sub(r'`[^`]+`', '', text)
    text = re.sub(r'```[^`]+```', '', text)


    text = re.sub(r'<[^>]+>', '', text)


    translator = str.maketrans(string.punctuation, ' ' * len(string.punctuation))
    text = text.translate(translator)


    text = text.lower()


    text = re.sub(r'\s+', ' ', text).strip()

    return text


def generate_ngrams(words: List[str], n: int) -> List[str]:
    """
    Генерирует n-граммы из списка слов.

    Args:
        words: Список слов
        n: Размер n-граммы

    Returns:
        Список n-грамм
    """
    return [' '.join(words[i:i+n]) for i in range(len(words) - n + 1)]


def get_top_tags(text: str, max_tags: int = 5) -> List[str]:
    """
    Выделяет наиболее важные теги из текста.

    Args:
        text: Исходный текст
        max_tags: Максимальное количество тегов

    Returns:
        Список строк-тегов
    """
    if not text or not text.strip():
        return []


    clean = clean_text(text)


    words = clean.split()


    filtered_words = [word for word in words if word not in STOP_WORDS and len(word) > 2]


    if not filtered_words:
        return []


    word_counts = Counter(filtered_words)


    bigrams = generate_ngrams(filtered_words, 2)
    bigram_counts = Counter(bigrams)


    candidates = []


    candidates.extend([word for word, _ in word_counts.most_common(10)])


    candidates.extend([bigram for bigram, _ in bigram_counts.most_common(5)])


    unique_tags: Set[str] = set()
    tags: List[str] = []


    for candidate in candidates:

        if candidate not in unique_tags and len(tags) < max_tags:
            tags.append(candidate)
            unique_tags.add(candidate)

    return tags
