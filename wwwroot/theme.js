// Файл для управления темой

// Функция применения темы
function applyTheme(theme) {
    if (theme === 'dark') {
        document.body.classList.add('dark-theme');
        document.body.classList.remove('light-theme');
        localStorage.setItem('theme', 'dark');
    } else {
        document.body.classList.add('light-theme');
        document.body.classList.remove('dark-theme');
        localStorage.setItem('theme', 'light');
    }
}

// Функция загрузки сохранённой темы
function loadTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        applyTheme(savedTheme);
    } else {
        // По умолчанию светлая тема
        applyTheme('light');
    }
}

// Функция переключения темы
function toggleTheme() {
    const currentTheme = localStorage.getItem('theme') || 'light';
    if (currentTheme === 'light') {
        applyTheme('dark');
    } else {
        applyTheme('light');
    }
}

// Загружаем тему при загрузке страницы
loadTheme();