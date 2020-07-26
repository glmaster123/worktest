const PREFIX = 'todoApp';

export default {
    set(key, data) {
        localStorage.setItem(`${PREFIX}.${key}`, JSON.stringify(data));
    },
    get(key, defaultValue) {
        let data = localStorage.getItem(`${PREFIX}.${key}`);

        if (data) {
            data = JSON.parse(data);
        } else {
            data = defaultValue;
        }

        return data;
    },
    delete(key) {
        localStorage.removeItem(`${PREFIX}.${key}`);
    },
    clear() {
        localStorage.clear();
    },
};