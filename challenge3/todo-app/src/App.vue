<template>
  <div id="app">
    <b-navbar
      type="dark"
      variant="info"
      sticky>
      <b-navbar-brand>Rogo Todo App</b-navbar-brand>
      <b-navbar-nav class="ml-auto">
        <b-form-checkbox
          size="lg"
          class="text-white"
          type="checkbox"
          v-model="online">
          Online
        </b-form-checkbox>
      </b-navbar-nav>
    </b-navbar>

    <!-- Existing Todo Items -->
    <b-list-group>
      <b-list-group-item
        v-for="todoItem in todoItems"
        :key="todoItem.id">
        <form
          @submit.prevent="updateToDoItem(todoItem)"
          class="d-flex align-items-center">
          <!-- Mark Todo Item As Completed -->
          <b-form-checkbox
            size="lg"
            type="checkbox"
            v-model="todoItem.isComplete"
            @input="updateToDoItem(todoItem)">
          </b-form-checkbox>
          <!--/ Mark Todo Item As Completed -->
          
          <!-- Todo Item Name -->
          <div
            class="flex-grow-1"
            :class="{
              completed: todoItem.isComplete,
            }">
            {{todoItem.name}}
          </div>
          <!-- / Todo Item Name -->
          
          <!-- Delete Todo Item -->
          <b-button
            variant="danger"
            type="button"
            @click="deleteToDoItem(todoItem)">
            <b-icon-trash></b-icon-trash>
          </b-button>
          <!-- / Delete Todo Item -->
        </form>
      </b-list-group-item>
      <!-- / Existing Todo Items -->
      
      <!-- Create Todo Item -->
      <b-list-group-item>
        <form
          @submit.prevent="createTodoItem"
          class="d-flex align-items-center">
          <b-form-input
            class="flex-grow-1 mr-3"
            v-model="newTodoItem.name"
            placeholder="Create a new todo item"
            autofocus>
          </b-form-input>
          <b-button
            variant="success"
            type="submit">
            <b-icon-plus></b-icon-plus>
          </b-button>
        </form>
      </b-list-group-item>
      <!-- / Create Todo Item -->
    </b-list-group>
  </div>
</template>

<script>
import axios from 'axios';
import storage from './storage';
import uuid from './uuid';

axios.defaults.baseURL = 'https://localhost:5001/api/';

export default {
  name: 'App',
  data() {
    return {
      todoItems: [],
      deletedToDoItemIDs: [],
      online: storage.get('online', true),
      newTodoItem: {
        name: '',
        isComplete: false,
      },
    };
  },
  watch: {
    online(isOnline) {
      if (isOnline) {
        this.sync();
      }

      storage.set('online', isOnline);
    },
  },
  methods: {
    createTodoItem() {
      const config = {
        url: 'TodoItems',
        method: 'POST',
        data: { ...this.newTodoItem },
      };
     
      if (this.online) {
        axios(config).then(() => {
          this.newTodoItem.name = '';
        })
        .catch(this.handleError)
        .finally(this.getTodoItems);
      } else {
        const todoItem = { ...this.newTodoItem };
        todoItem.id = uuid();
        todoItem.createdOffline = true;
        this.todoItems.push(todoItem);
        this.newTodoItem.name = '';
        storage.set('todoItems', this.todoItems);
      }
    },
    updateToDoItem(todoItem) {
      const config = {
        url: `TodoItems/${todoItem.id}`,
        method: 'PUT',
        data: { ...todoItem },
      };
     
      if (this.online) {
        axios(config)
          .catch(this.handleError)
          .finally(this.getTodoItems);
      } else {
        todoItem.updatedOffline = true;
        storage.set('todoItems', this.todoItems);
      }
    },
    deleteToDoItem(todoItem) {
      const config = {
        url: `TodoItems/${todoItem.id}`,
        method: 'DELETE',
      };
     
      if (this.online) {
        axios(config)
          .catch(this.handleError)
          .finally(this.getTodoItems);
      } else {
        this.todoItems = this.todoItems.filter((t) => t.id !== todoItem.id);
        this.deletedToDoItemIDs.push(todoItem.id);
        storage.set('todoItems', this.todoItems);
      }
    },
    getTodoItems() {
      return axios.get('TodoItems')
        .then(({ data }) => {
          this.todoItems = data;
          storage.set('todoItems', data);
        });
    },
    handleError({ response }) {
      if (response.status === 400 || response.status === 404) {
        alert('Data could not be synced because it is out of date. Refreshing todo items.');
      } else {
        alert('An unknown error occured');
      } 
    },
    sync() {
      const todoItemsToCreate = this.todoItems.filter((t) => t.createdOffline);
      const todoItemsToUpdate = this.todoItems.filter((t) => t.updatedOffline && !t.createdOffline);
      
      const deleteRequests = this.deletedToDoItemIDs.map((id) => {
        const config = {
          url: `TodoItems/${id}`,
          method: 'DELETE',
        };

        return axios(config);
      });

      const createRequests = todoItemsToCreate.map((t) => {
        const data = {
          name: t.name,
        };
        
        const config = {
          url: 'TodoItems',
          method: 'POST',
          data,
        };

        return axios(config);
      });

      const updateRequests = todoItemsToUpdate.map((t) => {
        const config = {
          url: `TodoItems/${t.id}`,
          method: 'PUT',
          data: t,
        };

        return axios(config);
      });
      
      return Promise.all([...deleteRequests, ...createRequests, ...updateRequests])
        .catch(this.handleError)
        .finally(this.getTodoItems);
    },
  },
  created() {
    if (this.online) {
      this.getTodoItems();
    } else {
      this.todoItems = storage.get('todoItems', []);
    }
  },
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}

.completed {
  text-decoration: line-through;
}
</style>
